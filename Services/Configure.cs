using System.Diagnostics;
using OnionFinder.Helpers;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Runtime.InteropServices;

using static Spectre.Console.AnsiConsole;

namespace OnionFinder.Services;

public static class Configure
{
    private static HttpClient _httpClient;
    private static string _torRoot;
    private static OSPlatform _os;
    private static string _downloadLink;
    private static string _downloadPath;

    public static async Task InstallAsync(HttpClient httpClient, string torRoot, string downloadLink)
    {
        _torRoot = torRoot;
        _httpClient = httpClient;
        _downloadLink = downloadLink;
        await ConfigureService();
    }
    
    private static async Task ConfigureService()
    {
        if (OperatingSystem.IsLinux())
        {
            await DownloadTor();

            ExtractTor();
            
            OptimizeTorrc();

            ImplementPrefs();

            ChangePermissions();

            
        }
        else if (OperatingSystem.IsWindows())
        {
            await DownloadTor();

            OptimizeTorrc();

            ImplementPrefs();
        }
    }
    private static async Task DownloadTor()
    {
        if (OperatingSystem.IsLinux())
        { 
            _downloadPath = Path.Combine(_torRoot, "tor.tar.xz");
            _ = await _httpClient.DownloadFileAsync($"https://www.torproject.org/{_downloadLink}", _downloadPath);
        }
        else if(OperatingSystem.IsWindows())
        {
            var downloadPath = Path.Combine(_torRoot, "tor.exe");

            _ = await _httpClient.DownloadFileAsync($"https://www.torproject.org/{_downloadLink}", _downloadPath);
        }
    }
    private static void ExtractTor()
    {
        using var stream = File.OpenRead(_downloadPath);

        var reader = ReaderFactory.Open(stream);

        while (reader.MoveToNextEntry())
        {
            if (!reader.Entry.IsDirectory)
            {
                reader.WriteEntryToDirectory(_torRoot, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
            }
        }
    }

    private static void OptimizeTorrc()
    {
        string torrcFile = Path.Combine(_torRoot, "tor-browser_en-US", "Browser", "TorBrowser", "Data", "Tor", "torrc");
        string[] optimizations = { "EntryNodes {US}", "ExitNodes {US}", "SocksPort 127.0.0.1:9050", "StrictNodes 1" };

        MarkupLine("Otimização de nodes realizado :check_mark:");

        foreach (var opt in optimizations)
        {
            using var streamWriter = File.AppendText(torrcFile);
            streamWriter.WriteLine(opt);
        }
    }
    
    private static void ImplementPrefs()
    {
        try
        {
            File.Copy(Path.Combine(_torRoot, "prefs.js"),
            Path.Combine(
                _torRoot, "tor-browser_en-US", "Browser", "TorBrowser", 
                "Data", "Browser", "profile.default", "prefs.js"));

        }
        catch (System.Exception)
        {
            WriteLine("Arquivo prefs.js já existe... continuando...");
        }
    }
    private static void ChangePermissions()
    {
        var chmod = Process.Start(new ProcessStartInfo 
                    { FileName = "/usr/bin/chmod", Arguments = "700 " + 
                    Path.Combine(_torRoot, "tor-browser_en-US") + " -R", UseShellExecute = true });
        chmod.WaitForExit();
        // This Sleep can allow chmod to change the permissions correctly
        Thread.Sleep(10);
    }
}
