using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Andtech.To
{

	internal class ShellUtility
	{

		public static void OpenBrowser(string url)
		{
			try
			{
				var browser = Environment.GetEnvironmentVariable("BROWSER");

				if (string.IsNullOrEmpty(browser))
				{
					Process.Start(url);
				}
				else
				{
					Process.Start(browser, url);
				}
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
}
