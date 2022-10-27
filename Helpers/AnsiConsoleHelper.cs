using Spectre.Console;

namespace OnionFinder.Helpers;

public static class AnsiConsoleHelper
{
  public static async Task AnsiStatusAsync(string status, Func<StatusContext, Task> action)
  {
    await AnsiConsole.Status()
                .SpinnerStyle(Style.Parse("white bold"))
                .Spinner(Spinner.Known.BouncingBall)
                .StartAsync(status, action);
  }

  public static void AnsiStatus(string status, Func<StatusContext, Task> action)
  {
    AnsiConsole.Status()
                .SpinnerStyle(Style.Parse("white bold"))
                .Spinner(Spinner.Known.BouncingBall)
                .Start(status, action);
  }
  public static void AnsiChart(int itemValue, string itemName, int itemValue2, string itemName2)
  {
    AnsiConsole.Write(new BreakdownChart()
        .Width(60)
        .AddItem(itemName, itemValue, Color.Purple)
        .AddItem(itemName2, itemValue2, Color.White)
    );
  }
}