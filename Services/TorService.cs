using Spectre.Console;
using OnionFinder.Models;
using System.Diagnostics;
using OnionFinder.Helpers;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Threading.Tasks.Dataflow;

using static Spectre.Console.AnsiConsole;
using static OnionFinder.Helpers.AnsiConsoleHelper;

namespace OnionFinder.Services;

public static class TorService
{
    private static string[] webEngines = 
    {
        "http://juhanurmihxlp77nkq76byazcldy2hlmovfu2epvl5ankdibsot4csyd.onion/search/?q="
    };

    private static List<string> tempUrls = new List<string>();
    private static List<string> sitesOnline = new List<string>();

    private static string torRoot = Path.Combine(System.AppContext.BaseDirectory, "requirements", "tor");
    private static string baseTorPath = Path.Combine(torRoot, "tor-browser_en-US", "Browser");
    private static string torPath = Path.Combine(baseTorPath, "start-tor-browser");
    private static string profilePath = Path.Combine(baseTorPath, "TorBrowser", "Data", "Browser", "profile.default");

    private static HttpClient _httpClient = new HttpClient(new SocketsHttpHandler()
    {
        Proxy = new System.Net.WebProxy("socks5://127.0.0.1:9050"),
        UseProxy = true
    });
    
    public static async Task LoadTor(OperatingSystems os = OperatingSystems.Linux)
    {
        if (File.Exists(TorService.torPath) == false || Directory.Exists(TorService.profilePath) == false)
        {
            await SetupTor(os);
        }

        if (TestTorProccess())
        {
            MarkupLine("Tor Browser iniciado :check_mark:");
            await TestProxy();
        };
    }
    
    private static bool TestTorProccess()
    {
        if (Process.GetProcessesByName("tor").Length == 0)
        {
            try
            {
                AnsiStatus("Iniciando Tor Browser...", ctx =>
                {
                    Process.Start(new ProcessStartInfo { FileName = torPath, Arguments = "--detach", UseShellExecute = true });
                    Thread.Sleep(5000);
                    return Task.CompletedTask;
                });
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        return true;
    }

    // todo: refator this code, again, yes
    private static async Task SetupTor(OperatingSystems os)
    {
        WriteLine();
        
        await AnsiStatusAsync("Baixando e configurando o Tor...", async ctx =>
        {
            var htmlDoc = await _httpClient.LoadHtmlDocument("https://www.torproject.org/download/");

            var downloadLink = string.Empty;
            var downloadDoc = htmlDoc.DocumentNode.SelectNodes("//a").Where(node => node.GetAttributeValue("class", "").Contains("btn btn-primary mt-4 downloadLink"));

            foreach (var node in downloadDoc)
            {
                if (node.GetAttributeValue("href", "").EndsWith(".tar.xz") ||
                    node.GetAttributeValue("href", "").EndsWith(".dmg") ||
                    node.GetAttributeValue("href", "").EndsWith(".exe"))
                {
                    downloadLink = node.GetAttributeValue("href", "");
                }
            }

            if (os == OperatingSystems.Linux)
            {
                //Download Linux version
                var downloadPath = Path.Combine(torRoot, "tor.tar.xz");

                _ = await _httpClient.DownloadFileAsync($"https://www.torproject.org/{downloadLink}", downloadPath);
                
                //Extract Linux version
                using var stream = File.OpenRead(downloadPath);

                var reader = ReaderFactory.Open(stream);

                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(torRoot, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                    }
                }

                string torrcFile = Path.Combine(torRoot, "tor-browser_en-US", "Browser", "TorBrowser", "Data", "Tor", "torrc");
                string[] optimizations = { "EntryNodes {US}", "ExitNodes {US}", "SocksPort 127.0.0.1:9050", "StrictNodes 1" };

                WriteLine("Optimizing Torrc file...");

                foreach (var opt in optimizations)
                {
                    using var streamWriter = File.AppendText(torrcFile);
                    streamWriter.WriteLine(opt);
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
                    WriteLine("Arquivo prefs.js já existe... continuando...");
                }
            }
        });
    }

    public static async Task RunSearch(string keyword)
    {
        await AnsiStatusAsync("Buscando links...", async ctx =>
        {
            foreach (var engine in webEngines)
            {
                var resultPage = await _httpClient.LoadHtmlDocument(engine + keyword);
                resultPage.FromAhmia();
            }
        });

        if (tempUrls.Count > 0)
        {
            MarkupLine($" Links encontrados: [bold][purple]{tempUrls.Count}[/][/]");
            await TestUrls(tempUrls);
        }
        else
        {
            MarkupLine($" Nenhum resultado foi encontrado.");
        }

        tempUrls.Clear();
        sitesOnline.Clear();
    }
    
    private static async Task<string> TestProxy()
    {
        var value = string.Empty;

        while (true)
        {
            try
            {
                await AnsiStatusAsync("Testando a conexão com o proxy...", async ctx =>
                {
                    value = await _httpClient.GetStringAsync("http://duckduckgo.com");
                });

                MarkupLine(" Proxy Tor conectado :check_mark:");
                break;
            }
            catch (Exception) 
            {
                Thread.Sleep(100);
            }
        };

        return value;
    }

    private static async Task TestUrls(List<string> urls)
    {
        try
        {
            WriteLine();

            var rule = new Rule("[purple]Resultados[/]");
                rule.Alignment = Justify.Left;

            Write(rule);
            
            await AnsiStatusAsync("Testando links...", ctx =>
            {
                var tester = new ActionBlock<string>(async url =>
                {
                    var htmlDoc = await _httpClient.LoadHtmlDocument(url);

                    var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title").InnerText;

                    title = title.Length > 50 ? title[..50] : title; 

                    var table = new Table()
                    {
                        Width = 80,
                        Border = TableBorder.Rounded,
                        BorderStyle = new Style(Color.Purple)
                    };
                    
                    table.AddColumn(new TableColumn(new Markup($"[bold]{title}[/]")));
                    table.AddRow(new Markup($"[link]{url}[/]"));

                    Write(table);
                    sitesOnline.Add(url);
                }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 100 });

                Parallel.ForEach(urls, (url) => tester.SendAsync(url));

                tester.Complete();
                tester.Completion.Wait();

                return Task.CompletedTask;
            });
        }
        catch (System.Exception) {}
    }
}