using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MonitorulOficialPDF.Web.Services;

namespace MonitorulOficialPDF.Web.Pages;

public class OcrViewModel : PageModel
{
    private readonly PdfDownloadService _pdfDownloadService;
    private readonly OcrService _ocrService;
    private readonly AzureOpenAiService _aiService;
    private readonly ILogger<OcrViewModel> _logger;

    public OcrViewModel(
        PdfDownloadService pdfDownloadService,
        OcrService ocrService,
        AzureOpenAiService aiService,
        ILogger<OcrViewModel> logger)
    {
        _pdfDownloadService = pdfDownloadService;
        _ocrService = ocrService;
        _aiService = aiService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true, Name = "url")]
    public new string? Url { get; set; }

    public string? ExtractedText { get; set; }
    public string? AiAnalysis { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        if (string.IsNullOrEmpty(Url))
        {
            ErrorMessage = "Nu a fost specificat un URL valid pentru document.";
            return;
        }

        try
        {
            var pdfBytes = await _pdfDownloadService.DownloadPdfAsync(Url);
            if (pdfBytes.Length == 0)
            {
                ErrorMessage = "Nu s-a putut descărca documentul PDF.";
                return;
            }

            ExtractedText = await _ocrService.ExtractTextAsync(pdfBytes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la extragerea textului OCR pentru {Url}", Url);
            ErrorMessage = "A apărut o eroare la extragerea textului. Vă rugăm încercați din nou.";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Url))
        {
            ErrorMessage = "Nu a fost specificat un URL valid pentru document.";
            return Page();
        }

        try
        {
            var pdfBytes = await _pdfDownloadService.DownloadPdfAsync(Url);
            if (pdfBytes.Length == 0)
            {
                ErrorMessage = "Nu s-a putut descărca documentul PDF.";
                return Page();
            }

            ExtractedText = await _ocrService.ExtractTextAsync(pdfBytes);
            AiAnalysis = await _aiService.AnalyzeTextAsync(ExtractedText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Eroare la analiza AI pentru {Url}", Url);
            ErrorMessage = "A apărut o eroare la analiza textului. Vă rugăm încercați din nou.";
        }

        return Page();
    }
}
