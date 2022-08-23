using Andtech.Common;
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
			Session.UseGlobalMode = options.UseGlobalMode;
			var session = Session.Load();

			var input = string.Join(" ", options.Tokens);
			var query = Query.Parse(input);

			var selector = new HotspotSelector(session.Hotspots);

			if (options.List)
			{
				var ranks = selector.Order(query);

				if (ranks.Any())
				{
					Log.WriteLine($"Q fuz H\tQ ex H\tAcc\tURL", Verbosity.silly);
					foreach (var rank in ranks.Take(5))
					{
						var accuracy = Math.Round(rank.Accuracy * 100);
						Log.WriteLine($"{rank.CountOfQueryKeywordsAreFuzzy}\t{rank.CountOfQueryKeywordsAreExact}\t{accuracy}%\t{rank.Hotspot.url}", Verbosity.silly);
					}
				}
				else
				{
					Log.WriteLine("No Matches", ConsoleColor.Red);
				}
			}
			else
			{
				if (selector.Find(query, out var best))
				{
					Open(best, query);
				}
				else
				{
					Log.WriteLine("No Matches", ConsoleColor.Red);
				}
			}
		}

		void Open(Hotspot hotspot, Query query)
		{
			string url;
			if (query.Search is null || string.IsNullOrEmpty(hotspot.searchURL))
			{
				url = hotspot.url;
			}
			else
			{
				var queryString = query.Search;
				queryString = HttpUtility.UrlEncode(queryString);
				url = Regex.Replace(hotspot.searchURL, "%{query}", queryString);
			}

			if (query.Path != null)
			{
				url = $"{url}/{query.Path}";
			}

			Log.WriteLine(url, ConsoleColor.Green);

			if (!options.DryRun)
			{
				ShellUtility.OpenBrowser(url);
			}
		}
	}
}
