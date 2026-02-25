using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MonitorulOficialPDF.Web.Services
{
    public class OcrService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OcrService> _logger;

        public OcrService(IConfiguration configuration, ILogger<OcrService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> ExtractTextAsync(byte[] pdfBytes)
        {
            var endpoint = _configuration["AzureDocumentIntelligence:Endpoint"];
            var key = _configuration["AzureDocumentIntelligence:Key"];

            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(key))
            {
                _logger.LogWarning("Azure Document Intelligence is not configured. Set AzureDocumentIntelligence:Endpoint and AzureDocumentIntelligence:Key in configuration.");
                return "OCR service is not configured. Please set Azure Document Intelligence endpoint and key.";
            }

            try
            {
                var credential = new AzureKeyCredential(key);
                var client = new DocumentAnalysisClient(new Uri(endpoint), credential);

                using var stream = new MemoryStream(pdfBytes);
                var operation = await client.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", stream);
                var result = operation.Value;

                var text = string.Join(Environment.NewLine,
                    result.Pages.SelectMany(p => p.Lines).Select(l => l.Content));

                _logger.LogInformation("Successfully extracted {Length} characters from PDF", text.Length);
                return text;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from PDF using Azure Document Intelligence");
                return $"Error during OCR processing: {ex.Message}";
            }
        }
    }
}
