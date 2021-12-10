using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace Andtech.To
{

	public struct Query
	{
		public string[] Keywords { get; set; }
		public string Path { get; set; }
		public string Search { get; set; }
	}

	class Program
	{

		static void Main(string[] tokens)
		{
			var input = string.Join(" ", tokens);
			var query = CreateQuery(input);

			var ranks = ReadHotspots()
				.Select(x => Rank.ToRank(x, query))
				.OrderByDescending(x => x.FuzzyMatchCount)
				.ThenByDescending(x => x.Accuracy)
				.ThenByDescending(x => x.Score);

			if (ranks.Any(x => x.FuzzyMatchCount > 0))
			{
				Open(ranks.First().Hotspot, query);
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("No matches");
				Console.ResetColor();
			}
		}

		static Query CreateQuery(string input)
		{
			var hotspotMatch = Regex.Match(input, @"^(?<value>[^/?]+)");
			var suffixMatch = Regex.Match(input, @"/(?<value>.+)");
			var searchMatch = Regex.Match(input, @"\?(?<value>.+)");

			var queryKeywords = Regex.Split(hotspotMatch.Groups["value"].Value, @"[\s]+")
				.Select(x => x.Trim())
				.ToArray();

			return new Query
			{
				Keywords = queryKeywords,
				Path = suffixMatch.Success ? suffixMatch.Groups["value"].Value.Trim() : null,
				Search = searchMatch.Success ? searchMatch.Groups["value"].Value.Trim() : null,
			};
		}

		static List<Hotspot> ReadHotspots()
		{
			var prefix = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			var toFile = Path.Combine(prefix, "to.json");
			var toDirectory = Path.Combine(prefix, ".to");

			List<string> toFiles = new List<string>();
			if (Directory.Exists(toDirectory))
			{
				var files = Directory.EnumerateFiles(toDirectory, "*.json", SearchOption.AllDirectories);
				toFiles.AddRange(files);
			}
			else if (File.Exists(toFile))
			{
				toFiles.Add(toFile);
			}

			return toFiles.SelectMany(ReadHotspot).ToList();

			Hotspot[] ReadHotspot(string path)
			{
				var content = File.ReadAllText(path);

				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				return JsonSerializer.Deserialize<Hotspot[]>(content, options);
			}
		}

		static void Open(Hotspot hotspot, Query query)
		{
			string url;
			if (query.Search is null || string.IsNullOrEmpty(hotspot.SearchURL))
			{
				url = hotspot.URL;
			}
			else
			{
				var queryString = query.Search;
				queryString = HttpUtility.UrlEncode(queryString);
				url = Regex.Replace(hotspot.SearchURL, "%{query}", queryString);
			}

			if (query.Path != null)
			{
				url = $"{url}/{query.Path}";
			}

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(url);
			Console.ResetColor();

			ShellUtility.OpenBrowser(url);
		}
	}
}

