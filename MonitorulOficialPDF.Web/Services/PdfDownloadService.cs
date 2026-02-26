using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace MonitorulOficialPDF.Web.Services
{
    public class PdfDownloadService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PdfDownloadService> _logger;

        public PdfDownloadService(IHttpClientFactory httpClientFactory, IMemoryCache cache, ILogger<PdfDownloadService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _logger = logger;
        }

        public async Task<byte[]> DownloadPdfAsync(string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl) ||
                !relativeUrl.StartsWith("/") ||
                relativeUrl.StartsWith("//") ||
                relativeUrl.Contains("@") ||
                relativeUrl.Contains("\\"))
            {
                _logger.LogWarning("Invalid relative URL rejected: {Url}", relativeUrl);
                return Array.Empty<byte>();
            }

            if (_cache.TryGetValue(relativeUrl, out byte[]? cachedPdf) && cachedPdf != null)
            {
                _logger.LogInformation("Returning cached PDF for {Url}", relativeUrl);
                return cachedPdf;
            }

            try
            {
                var client = _httpClientFactory.CreateClient("PdfDownloader");
                var response = await client.GetAsync("https://monitoruloficial.ro" + relativeUrl);
                response.EnsureSuccessStatusCode();

                var pdfBytes = await response.Content.ReadAsByteArrayAsync();

                _cache.Set(relativeUrl, pdfBytes, TimeSpan.FromMinutes(30));
                _logger.LogInformation("Downloaded and cached PDF for {Url}", relativeUrl);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading PDF from {Url}", relativeUrl);
                return Array.Empty<byte>();
            }
        }
    }
}
