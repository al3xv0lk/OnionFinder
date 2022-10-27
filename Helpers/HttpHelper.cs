using HtmlAgilityPack;

namespace OnionFinder.Helpers;

public static class HttpHelper
{
    public static async Task<byte[]> DownloadFileAsync(this HttpClient httpClient, string uri, string path)
    {
        var response = await httpClient.GetAsync(uri);
        
        using var fileStream = new FileStream(path, FileMode.CreateNew);

        await response.Content.CopyToAsync(fileStream);

        return await response.Content.ReadAsByteArrayAsync();
    }

    public static async Task<HtmlDocument> LoadHtmlDocument(this HttpClient httpClient, string uri)
    {
        var htmlDocument = new HtmlDocument();

        htmlDocument.LoadHtml(await httpClient.GetStringAsync(uri));

        return htmlDocument;
    }
}
