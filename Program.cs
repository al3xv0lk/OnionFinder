using Spectre.Console;

using static Spectre.Console.AnsiConsole;
using static OnionFinder.Services.TorService;
using static OnionFinder.Helpers.ConsoleHelper;

public class Program
{
    public static async Task Main()
    {
        await Start();

        while (true)
        {
            WhiteSpace();

            Write(new FigletText("OnionFinder").Alignment(Justify.Left).Color(Color.Purple));

            WhiteSpace();

            var panel = new Panel("Essa ferramenta foi feita para facilitar suas buscas na rede Tor(Deep Web).\n[bold]Atenção:[/] 99% dos sites que pedem algum tipo de pagamento, são golpes.\n[bold]Lembre-se[/]: [italic]você é o único responsável por suas ações.[/]")
            {
                Header = new PanelHeader("Bem-vindo(a)!", Justify.Center),
                Padding = new Padding(1),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Purple)
            };

            Write(panel);
            WhiteSpace(3);

            while (true)
            {
                try
                {
                    WhiteSpace();

                    var keyword = Ask<string>("[bold] Pesquisar:[/]").Replace(" ", "+").EscapeMarkup();

                    await RunSearch(keyword);
                    WhiteSpace();

                    var ruleEnd = new Rule("[purple]Fim dos resultados[/]");
                        ruleEnd.Alignment = Justify.Left;

                    Write(ruleEnd);
                    WhiteSpace(3);
                }
                catch (TaskCanceledException) {}
                catch (HttpRequestException)
                {
                    break;
                }
            }
        }
    }

    private static async Task Start()
    {
        if (OperatingSystem.IsLinux())
        {
            await LoadTor();
        }
    }
}