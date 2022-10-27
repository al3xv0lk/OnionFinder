using System.Diagnostics;
using System.Net;
using System.Threading.Tasks.Dataflow;
using HtmlAgilityPack;
using SharpCompress.Readers;
using SharpCompress.Common;
using Spectre.Console;

public class Program
{
  private static string[] webEngines = 
  {
    "http://juhanurmihxlp77nkq76byazcldy2hlmovfu2epvl5ankdibsot4csyd.onion/search/?q="
  };
  private static HashSet<string> tempUrls = new();
  private static HashSet<string> sitesOnline = new();
  private static string torRoot = Path.Combine(System.AppContext.BaseDirectory, "requirements", "tor");
  private static string baseTorPath = Path.Combine(torRoot, "tor-browser_en-US", "Browser");
  private static string torPath = Path.Combine(baseTorPath, "start-tor-browser");
  private static string profilePath = Path.Combine(baseTorPath, "TorBrowser", "Data", "Browser", "profile.default");

  public static async Task Main()
  {
    var http = new HttpClient();
    if (OperatingSystem.IsLinux())
    {
      if (File.Exists(torPath) == false || Directory.Exists(profilePath) == false)
      {
        await SetupTor(1, http);
      }
    };
    http.Dispose();
    var proxy = new WebProxy("socks5://127.0.0.1:9050");
    http = new HttpClient(new SocketsHttpHandler
    {
      Proxy = proxy,
      UseProxy = true
    });
    while (true)
    {
      WhiteSpace(1);
      AnsiConsole.Write(
        new FigletText("OnionFinder")
            .Alignment(Justify.Left)
            .Color(Color.Purple));
      WhiteSpace(1);
      var panel = new Panel("Essa ferramenta foi feita para facilitar suas buscas na rede Tor(Deep Web).\n[bold]Atenção:[/] 99% dos sites que pedem algum tipo de pagamento, são golpes.\n[bold]Lembre-se[/]: [italic]você é o único responsável por suas ações.[/]");
      panel.Header = new PanelHeader("Bem-vindo(a)!", Justify.Center);
      panel.BorderColor(Color.Purple);
      panel.Border = BoxBorder.Rounded;
      AnsiConsole.Write(panel);
      WhiteSpace(3);
      if (TestTorProc(torPath))
      {
        AnsiConsole.MarkupLine(" Tor Browser iniciado :check_mark:");
        await TestProxy(http);
      };
      while (true)
      {
        try
        {
          System.Console.WriteLine();
          var keyword = AnsiConsole.Ask<string>("[bold] Pesquisar:[/]")
            .Replace(" ", "+")
            .EscapeMarkup();
          await RunSearch(http, keyword);
          System.Console.WriteLine();
          var ruleEnd = new Rule("[purple]Fim dos resultados[/]");
          ruleEnd.Alignment = Justify.Left;
          AnsiConsole.Write(ruleEnd);
          WhiteSpace(3);
        }
        catch (TaskCanceledException)
        {
        }
        catch (HttpRequestException)
        {
          break;
        }
        break;
      }
    }
  }

  // Just a shortcut to control the white space for aesthetics 
  private static void WhiteSpace(int n)
  {
    for (int i = 0; i <= n; i++)
    {
      Console.WriteLine();
    }
  }
  // Check if the Tor Browser is running as he is necessary, if not, run it .
  private static bool TestTorProc(string torPath)
  {
    Process[] processes = Process.GetProcessesByName("tor");
    if (processes.Length == 0)
    {
      try
      {
        AnsiConsole.Status()
          .SpinnerStyle(Style.Parse("white bold"))
          .Spinner(Spinner.Known.BouncingBall)
          .Start("Iniciando Tor Browser...", ctx =>
          {
            Process.Start(new ProcessStartInfo { FileName = torPath, Arguments = "--detach", UseShellExecute = true });
            Thread.Sleep(5000);
          });
      }
      catch (System.Exception)
      {
        throw;
      }
    }
    return true;
  }
  // Test proxy connection
  private static async Task<string> TestProxy(HttpClient http)
  {
    string value = "";
    while (true)
    {
      try
      {
        await AnsiConsole.Status()
            .SpinnerStyle(Style.Parse("white bold"))
            .Spinner(Spinner.Known.BouncingBall)
            .StartAsync("Testando a conexão com o proxy...", async ctx =>
          {
            value = await http.GetStringAsync("http://duckduckgo.com");
          });
        AnsiConsole.MarkupLine(" Proxy Tor conectado :check_mark:");
        break;
      }
      catch (Exception)
      {
        Thread.Sleep(100);
      }
    };
    return value;
  }
  // Download and config everything tor related
  private static async Task SetupTor(int os, HttpClient client)
  {
    System.Console.WriteLine();
    // System.Console.WriteLine(Directory.GetCurrentDirectory());
    await AnsiConsole.Status()
      .SpinnerStyle(Style.Parse("white bold"))
      .Spinner(Spinner.Known.BouncingBall)
      .StartAsync("Baixando e configurando o Tor...", async ctx =>
      {
        var data = await client.GetStringAsync("https://www.torproject.org/download/");
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(data);
        var downloadDoc = htmlDoc.DocumentNode.SelectNodes("//a")
          .Where(node => node.GetAttributeValue("class", "").Contains("btn btn-primary mt-4 downloadLink"));
        string downloadLink = "";
        foreach (var node in downloadDoc)
        {
          if (node.GetAttributeValue("href", "").EndsWith(".tar.xz"))
          {
            downloadLink = node.GetAttributeValue("href", "");
          }
          else if (node.GetAttributeValue("href", "").EndsWith(".dmg"))
          {
            downloadLink = node.GetAttributeValue("href", "");
          }
          else if (node.GetAttributeValue("href", "").EndsWith(".exe"))
          {
            downloadLink = node.GetAttributeValue("href", "");
          }
        }
        WebClient webClient = new WebClient();
        if (os == 1)
        {
          //Download Linux version
          webClient.DownloadFile("https://www.torproject.org/" + downloadLink,
          Path.Combine(torRoot, "tor.tar.xz"));
          //Extract Linux version
          using (Stream stream = File.OpenRead(
            Path.Combine(torRoot, "tor.tar.xz")))
          {
            var reader = ReaderFactory.Open(stream);
            while (reader.MoveToNextEntry())
            {
              if (!reader.Entry.IsDirectory)
              {
                reader.WriteEntryToDirectory(torRoot, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
              }
            }
          }
          string torrcFile = Path.Combine(torRoot, "tor-browser_en-US", "Browser", "TorBrowser", "Data", "Tor", "torrc");
          string[] optimizations = { "EntryNodes {US}", "ExitNodes {US}", "SocksPort 127.0.0.1:9050", "StrictNodes 1" };
          System.Console.WriteLine("Optimizing Torrc file...");
          foreach (var opt in optimizations)
          {
            using (StreamWriter sw = File.AppendText(torrcFile))
            {
              sw.WriteLine(opt);
            }
          }
          try
          {
            File.Copy(Path.Combine(torRoot, "prefs.js"),
            Path.Combine(torRoot, "tor-browser_en-US", "Browser", "TorBrowser", "Data", "Browser", "profile.default", "prefs.js"));
            var chmod = Process.Start(new ProcessStartInfo { FileName = "/usr/bin/chmod", Arguments = "700 " + Path.Combine(torRoot, "tor-browser_en-US") + " -R", UseShellExecute = true });
            chmod.WaitForExit();
          }
          catch (System.Exception)
          {
            System.Console.WriteLine("Arquivo prefs.js já existe... continuando...");
          }
        }
      });
  }
  // The algorithm that will execute the search
  private static async Task RunSearch(HttpClient http, string keyword)
  {
    await AnsiConsole.Status()
            .SpinnerStyle(Style.Parse("white bold"))
            .Spinner(Spinner.Known.BouncingBall)
            .StartAsync("Buscando links...", async ctx =>
          {
            foreach (var engine in webEngines)
            {
              var resultPage = await Search(http, engine, keyword);
              FromAhmia(resultPage);
            }
          });
    if (tempUrls.Count > 0)
    {
      AnsiConsole.MarkupLine($" Links encontrados: [bold][purple]{tempUrls.Count}[/][/]");
      await TestUrls(http, tempUrls);
    }
    else
    {
      AnsiConsole.MarkupLine($" Nenhum resultado foi encontrado.");
    }
    tempUrls.Clear();
    sitesOnline.Clear();
  }

  // Load the page results from Ahmia and parse it
  private static void FromAhmia(HtmlDocument htmlDoc)
  {
    try
    {
      var links = htmlDoc.DocumentNode.SelectNodes("//cite");
      if (links.Count > 0)
      {
        foreach (var node in links)
        {
          string endUrl = "http://" + node.InnerHtml;
          tempUrls.Add(endUrl);
        }
      }
    }
    catch (NullReferenceException)
    {
      System.Console.WriteLine("No results where found. Be aware that some terms are blocked by the Web Engine itself");
    }
    catch (Exception e)
    {
      System.Console.WriteLine(e);
    }
  }
  // Search in the defined WebEngine and return the complete html document
  private static async Task<HtmlDocument> Search(HttpClient client, string webEngine, string keyword)
  {
    var html = await client.GetStringAsync(webEngine + keyword);
    var htmlDoc = new HtmlDocument();
    htmlDoc.LoadHtml(html);
    return htmlDoc;
  }

  // Try to access the URL, if Online, print the title and link
  public static async Task TestUrls(HttpClient client, HashSet<string> urls)
  {
    try
    {
      Console.WriteLine();
      var rule = new Rule("[purple]Resultados[/]");
      rule.Alignment = Justify.Left;
      AnsiConsole.Write(rule);
      await AnsiConsole.Status()
            .SpinnerStyle(Style.Parse("white bold"))
            .Spinner(Spinner.Known.BouncingBall)
            .StartAsync("Testando links...", ctx =>
            {
              var tester = new ActionBlock<string>(async url =>
                {
                  var data = await client.GetStringAsync(url);
                  var htmlDoc = new HtmlDocument();
                  htmlDoc.LoadHtml(data);
                  var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;
                  if (title.Length > 50)
                  {
                    title = title.Substring(0, 50);
                  }
                  var table = new Table();
                  table.Width(80);
                  table.Border(TableBorder.Rounded);
                  table.BorderColor(Color.Purple);
                  table.AddColumn(new TableColumn(new Markup($"[bold]{title}[/]")));
                  table.AddRow(new Markup($"[link]{url}[/]"));
                  AnsiConsole.Write(table);
                  sitesOnline.Add(url);
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100 });
              Parallel.ForEach(urls, (url) =>
              {
                tester.SendAsync(url);
              });
              tester.Complete();
              tester.Completion.Wait();
              return Task.CompletedTask;
            });
    }
    catch (System.Exception)
    {
    }
  }
}