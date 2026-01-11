using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ycopy.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        var savedUrl = Request.Cookies["SavedUrl"];
        ViewData["SavedUrl"] = savedUrl;
    }

    public IActionResult OnPost()
    {
        var url = Request.Form["url"].ToString();

        if (!string.IsNullOrEmpty(url))
        {
            Response.Cookies.Append("SavedUrl", url, new CookieOptions
            {
                MaxAge = TimeSpan.MaxValue
            });
        }
        return RedirectToPage();
    }
}
