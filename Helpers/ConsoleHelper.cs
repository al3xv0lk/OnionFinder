using Spectre.Console;
using static Spectre.Console.AnsiConsole;


namespace OnionFinder.Helpers;

public static class ConsoleHelper
{
    public static void WhiteSpace(int n = 1)
    {
        for (int i = 0; i <= n; i++)
        {
            System.Console.WriteLine();
        }
    }

    public static void endOfResultsRule()
    {
        var ruleEnd = new Rule("[purple]Fim dos resultados[/]");
        ruleEnd.Alignment = Justify.Left;
        WhiteSpace();
        Write(ruleEnd);
        WhiteSpace(1);
    }
}