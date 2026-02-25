using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using MonitorulOficialPDF.Web.Models;
using System.Text;

namespace MonitorulOficialPDF.Web.Services
{
    public class MonitorScraperService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MonitorScraperService> _logger;

        public MonitorScraperService(IHttpClientFactory httpClientFactory, ILogger<MonitorScraperService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<PublicationInfo>> GetRecentPublicationsAsync()
        {
            var publications = new List<PublicationInfo>();

            for (int i = 0; i < 7; i++)
            {
                var date = DateTime.Now.AddDays(-i);
                var dateString = date.ToString("yyyy - MM - dd");

                try
                {
                    var client = _httpClientFactory.CreateClient("MonitorScraper");
                    var content = new StringContent("today=" + dateString, Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = await client.PostAsync("ramo_customs/emonitor/get_mo.php", content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var links = ExtractLinks(responseContent, dateString);
                    publications.AddRange(links);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error scraping publications for date {Date}", dateString);
                }
            }

            return publications;
        }

        private List<PublicationInfo> ExtractLinks(string html, string date)
        {
            var results = new List<PublicationInfo>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var anchorNodes = htmlDoc.DocumentNode.SelectNodes("//a");
            if (anchorNodes == null)
                return results;

            foreach (var item in anchorNodes)
            {
                var hrefValue = item.GetAttributeValue("href", string.Empty);

                if (hrefValue == "#")
                    continue;

                results.Add(new PublicationInfo
                {
                    Name = item.InnerText.Trim(),
                    Url = hrefValue,
                    Date = date
                });
            }

            return results;
        }
    }
}
