using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

class Program
{

    static void Main(string[] tokens)
    {
        var query = string.Join(" ", tokens);

        if (!TryLaunch(query))
        {
            Console.Error.WriteLine($"No results for '{query}'");
        }
    }

    public static bool TryLaunch(string query)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "hotspots.csv");
        var content = File.ReadAllLines(path);
        foreach (var line in content)
        {
            try
            {
                var values = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                var name = values[0];
                var url = values[1];

                if (name == query)
                {
                    OpenBrowser(url);
                    return true;
                }
            }
            catch { }
        }

        return false;
    }

    public static void OpenBrowser(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
}
