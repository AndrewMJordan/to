using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Andtech.To
{

	internal class Runner
	{
		private readonly Options options;

		public Runner(Options options)
		{
			this.options = options;
		}

		public async Task Run()
		{
			var input = string.Join(" ", options.Tokens);
			var query = Query.Parse(input);

			var session = Session.Load();

			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			if (options.List)
			{
				foreach (var rank in ranks.Take(5))
				{
					Console.WriteLine($"{rank.FuzzyMatchCount}\t{rank.Accuracy}\t{rank.Hotspot.URL}");
				}
			}

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

		void Open(Hotspot hotspot, Query query)
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

			if (!options.DryRun)
			{
				ShellUtility.OpenBrowser(url);
			}
		}
	}
}
