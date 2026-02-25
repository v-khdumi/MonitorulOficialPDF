using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitorulOficialPDF.Web.Services;

namespace MonitorulOficialPDF.Web.Pages.Api;

public class PdfModel : PageModel
{
    private readonly PdfDownloadService _pdfDownloadService;
    private readonly ILogger<PdfModel> _logger;

    public PdfModel(PdfDownloadService pdfDownloadService, ILogger<PdfModel> logger)
    {
        _pdfDownloadService = pdfDownloadService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync([FromQuery] string? url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("Parametrul 'url' este obligatoriu.");
        }

        try
        {
            var pdfBytes = await _pdfDownloadService.DownloadPdfAsync(url);
            if (pdfBytes.Length == 0)
            {
                return NotFound("Documentul PDF nu a putut fi descărcat.");
            }

            return File(pdfBytes, "application/pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la descărcarea PDF-ului de la {Url}", url);
            return StatusCode(500, "Eroare internă la descărcarea documentului.");
        }
    }
}
