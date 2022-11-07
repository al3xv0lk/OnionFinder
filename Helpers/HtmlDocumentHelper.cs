using HtmlAgilityPack;

namespace OnionFinder.Helpers;

public static class HtmlDocumentHelper
{
    public static List<string> FromAhmia(this HtmlDocument htmlDoc)
    {
        try
        {
            var tempUrls = new List<string>();
            var links = htmlDoc.DocumentNode.SelectNodes("//cite");

            if (links.Count > 0)
            {
                foreach (var node in links)
                {
                    var endUrl = "http://" + node.InnerHtml;
                    tempUrls.Add(endUrl);
                }
            }
            System.Console.WriteLine(tempUrls.Count);
            return tempUrls;
        }
        catch (NullReferenceException)
        {
            System.Console.WriteLine("No results were found. Be aware that some terms are blocked by the Web Engine itself.");
            return default;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e);
            return default;
        }
    }
}