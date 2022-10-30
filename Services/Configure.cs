using System.Diagnostics;
using OnionFinder.Helpers;
using SharpCompress.Common;
using SharpCompress.Readers;
using System.Runtime.InteropServices;

using static Spectre.Console.AnsiConsole;

namespace OnionFinder.Services;

public static class Configure
{
    public static async Task InstallAsync(HttpClient httpClient, string torRoot, string downloadLink)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) 
        {
            await ConfigureService(httpClient, OSPlatform.Windows, torRoot, downloadLink);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
        {
            await ConfigureService(httpClient, OSPlatform.Linux, torRoot, downloadLink);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) 
        {
            await ConfigureService(httpClient, OSPlatform.OSX, torRoot, downloadLink);
        }
    }

    private static async Task ConfigureService(HttpClient httpClient, OSPlatform os, string torRoot, string downloadLink)
    {
        if (os == OSPlatform.Linux)
        {
            //Download Linux version
            var downloadPath = Path.Combine(torRoot, "tor.tar.xz");

            _ = await httpClient.DownloadFileAsync($"https://www.torproject.org/{downloadLink}", downloadPath);
            
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

            MarkupLine("Otimização de nodes realizado :check_mark:");

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
                // This Sleep can allow chmod to change the permissions correctly
                Thread.Sleep(10);
            }
            catch (System.Exception)
            {
                WriteLine("Arquivo prefs.js já existe... continuando...");
            }
        }
    }
}
