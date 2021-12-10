using System;
using System.Linq;
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

				Console.WriteLine($"Q fuz H\tQ ex H\tAcc\tURL");
				foreach (var rank in ranks.Take(5))
				{
					var accuracy = Math.Round(rank.Accuracy * 100);
					Console.WriteLine($"{rank.CountOfQueryKeywordsAreFuzzy}\t{rank.CountOfQueryKeywordsAreExact}\t{accuracy}%\t{rank.Hotspot.URL}");
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
