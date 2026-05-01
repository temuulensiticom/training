using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DbSyncTool.Pages
{
    // Minimal PageModel for the Index page.
    // The WinForms MainForm implementation belonged in a different folder and
    // caused duplicate type/name and missing reference errors when left here.
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
