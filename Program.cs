using Spectre.Console;
using static Spectre.Console.AnsiConsole;
using static OnionFinder.Services.TorService;
using static OnionFinder.Helpers.ConsoleHelper;
using static OnionFinder.Helpers.Messages;


while (true)
{
    await LoadTor();

    WelcomeMsg();

    while (true)
    {
        try
        {
            var keyword = Ask<string>("[bold] Pesquisar:[/]").Replace(" ", "+").EscapeMarkup();

            await RunSearch(keyword);

            endOfResultsRule();
        }
        catch (TaskCanceledException) { }
        catch (HttpRequestException)
        {
            break;
        }
    }
}
