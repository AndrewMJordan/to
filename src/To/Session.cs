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

			var path = Environment.GetEnvironmentVariable("TOPATH");
			var directories = string.IsNullOrEmpty(path) ? Enumerable.Empty<string>() : path.Split(PathDelimiter, StringSplitOptions.RemoveEmptyEntries);

			foreach (var directory in directories)
			{
				var files = Directory.EnumerateFiles(directory, "*.json", SearchOption.TopDirectoryOnly)
					.Concat(Directory.EnumerateFiles(directory, "*.yaml", SearchOption.TopDirectoryOnly));
				hotspots.AddRange(files.SelectMany(Hotspot.ReadMany));
			}

			return new Session
			{
				Hotspots = hotspots.ToArray()
			};
		}
	}
}
