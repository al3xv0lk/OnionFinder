using Spectre.Console;
using static Spectre.Console.AnsiConsole;
using static OnionFinder.Helpers.ConsoleHelper;

namespace OnionFinder.Helpers;
public class Messages
{
    public static void WelcomeMsg()
    {
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
    }
}