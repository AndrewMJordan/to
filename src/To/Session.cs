using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Andtech.To
{

	public class Session
	{
		public Hotspot[] Hotspots { get; set; }

		private static readonly char PathDelimiter = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ';' : ':';

		public static Session Load()
		{
			var hotspots = new List<Hotspot>();

			var path = Environment.GetEnvironmentVariable("ANDTECH_TO_PATH", EnvironmentVariableTarget.Process);
			var directories = string.IsNullOrEmpty(path) ? Enumerable.Empty<string>() : path.Split(PathDelimiter, StringSplitOptions.RemoveEmptyEntries);
			var toFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "to.json");

			if (File.Exists(toFile))
			{
				hotspots.AddRange(Hotspot.Read(toFile));
			}

			foreach (var directory in directories)
			{
				var files = Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories);
				hotspots.AddRange(files.SelectMany(Hotspot.Read));
			}

			return new Session
			{
				Hotspots = hotspots.ToArray()
			};
		}
	}
}
