using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitorulOficialPDF.Web.Models;
using MonitorulOficialPDF.Web.Services;

namespace MonitorulOficialPDF.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MonitorScraperService _scraperService;

    public IndexModel(ILogger<IndexModel> logger, MonitorScraperService scraperService)
    {
        _logger = logger;
        _scraperService = scraperService;
    }

    public Dictionary<string, List<PublicationInfo>> GroupedPublications { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var publications = await _scraperService.GetRecentPublicationsAsync();
            GroupedPublications = publications
                .GroupBy(p => p.Date)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la încărcarea publicațiilor");
            ErrorMessage = "A apărut o eroare la încărcarea publicațiilor. Vă rugăm încercați din nou.";
        }
    }
}
