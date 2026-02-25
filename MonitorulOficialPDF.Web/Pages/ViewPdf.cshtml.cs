using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MonitorulOficialPDF.Web.Pages;

public class ViewPdfModel : PageModel
{
    [BindProperty(SupportsGet = true, Name = "url")]
    public new string? Url { get; set; }

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        if (string.IsNullOrEmpty(Url))
        {
            ErrorMessage = "Nu a fost specificat un URL valid pentru document.";
        }
    }
}
