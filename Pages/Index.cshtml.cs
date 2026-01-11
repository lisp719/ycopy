using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using System.Xml.Linq;

namespace ycopy.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        var savedUrl = Request.Cookies["SavedUrl"];
        var rssUrl = ConvertToRssUrl(savedUrl ?? "");

        ViewData["SavedUrl"] = savedUrl;

        if (!string.IsNullOrEmpty(rssUrl))
        {
            ViewData["Entries"] = await GetFeedEntriesAsync(rssUrl);
        }
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

    private async Task<List<(string Title, string Url)>> GetFeedEntriesAsync(string url)
    {
        try
        {
            using var client = new HttpClient();

            var response = await client.GetStringAsync(url);
            var doc = XDocument.Parse(response);
            var entries = doc.Descendants(XName.Get("entry", "http://www.w3.org/2005/Atom"));

            return entries.Select(e => (
                Title: e.Element(XName.Get("title", "http://www.w3.org/2005/Atom"))?.Value ?? "タイトルなし",
                Url: e.Element(XName.Get("link", "http://www.w3.org/2005/Atom"))?.Attribute("href")?.Value ?? "#"
            )).ToList();
        }
        catch
        {
            return new List<(string Title, string Url)> { ("フィード取得エラー", "#") };
        }
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
