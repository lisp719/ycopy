using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

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
        var rssUrl = ConvertToRssUrl(url);

        if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(rssUrl))
        {
            Response.Cookies.Append("SavedUrl", rssUrl, new CookieOptions
            {
                MaxAge = TimeSpan.MaxValue
            });
        }
        return RedirectToPage();
    }

    private string ConvertToRssUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            uri.Host == "www.youtube.com" &&
            uri.AbsolutePath == "/playlist")
        {
            var query = QueryHelpers.ParseQuery(uri.Query);
            if (query.TryGetValue("list", out var playlistId))
            {
                return $"https://www.youtube.com/feeds/videos.xml?playlist_id={playlistId}";
            }
        }
        return "";
    }
}
