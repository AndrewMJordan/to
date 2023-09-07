using Andtech.Common;
using Andtech.Common.Text.SentenceExpressions;
using System;
using System.IO;
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
			var query = Query.Parse(input.ToLower());
			var selector = new HotspotSelector(session.Hotspots);

			if (options.List)
			{
				List();
			}
			else
			{
				Search();
			}

			void List()
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

			void Search()
			{
				// Try README
				if (!options.UseGlobalMode)
				{
					if (SearchReadme(query))
					{
						return;
					}
				}

				// Fallback
				if (selector.Find(query, out var best))
				{
					Open(best, query);
					return;
				}

				Log.WriteLine("No Matches", ConsoleColor.Red);
			}
		}

		class HotspotOption : ISearchable
		{
			public string Text { get; private set; }
			public Hotspot Hotspot { get; private set; }

			public HotspotOption(string key, Hotspot value)
			{
				Text = key;
				Hotspot = value;
			}

		}
		bool SearchReadme(Query query)
		{
			if (ShellUtil.Find(Environment.CurrentDirectory, "README.md", out var path, FindOptions.RecursiveUp))
			{

				var text = File.ReadAllText(path);
				var regex = new Regex(@"\[(?<title>[\w\s]+)]\((?<url>\w+://.+)\)");

				var matches = regex.Matches(text);
				var options = matches
					.Select(ToHotspot)
					.Select(x => new HotspotOption(x.name, x));

				Hotspot ToHotspot(System.Text.RegularExpressions.Match match)
				{
					return new Hotspot()
					{
						name = match.Groups["title"].Value.ToLower(),
						url = match.Groups["url"].Value,
					};
				}

				var search = new RankedSearch<HotspotOption>(query.ToString());
				if (search.Search(options, out var best))
				{
					Open(best.Hotspot, query);
					return true;
				}
			}

			return false;
		}

		void Open(Hotspot hotspot, Query query)
		{
			string url;
			if (query.SearchTerm is null || string.IsNullOrEmpty(hotspot.searchURL))
			{
				url = hotspot.url;
			}
			else
			{
				var queryString = query.SearchTerm;
				queryString = HttpUtility.UrlEncode(queryString);
				url = Regex.Replace(hotspot.searchURL, "%{query}", queryString);
			}

			if (query.SubPath != null)
			{
				url = $"{url}/{query.SubPath}";
			}

			Log.WriteLine(url, ConsoleColor.Green);

			if (!string.IsNullOrEmpty(hotspot.executable))
			{
				Environment.SetEnvironmentVariable("BROWSER", hotspot.executable);
			}

			if (!options.DryRun)
			{
				ShellUtil.OpenBrowser(url);
			}
		}
	}
}
