using Spectre.Console;

using static Spectre.Console.AnsiConsole;
using static OnionFinder.Services.TorService;
using static OnionFinder.Helpers.ConsoleHelper;
using static OnionFinder.Helpers.Messages;

public class Program
{
    public static async Task Main()
    {
        while (true)
        {
            await LoadTor();
            
            WelcomeMsg();

            while (true)
            {
                try
                {
                    WhiteSpace();

                    var keyword = Ask<string>("[bold] Pesquisar:[/]").Replace(" ", "+").EscapeMarkup();
                    await RunSearch(keyword);

                    var ruleEnd = new Rule("[purple]Fim dos resultados[/]");
                    ruleEnd.Alignment = Justify.Left;
                    WhiteSpace();
                    Write(ruleEnd);
                    WhiteSpace(1);
                }
                catch (TaskCanceledException) {}
                catch (HttpRequestException)
                {
                    break;
                }
            }
        }
    }
}
