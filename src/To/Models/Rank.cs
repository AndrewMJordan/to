using FuzzySharp;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.To
{

	public class Rank
	{
		public Hotspot Hotspot { get; private set; }
		public string[] Keywords { get; private set; }
		public int PrefixMatchCount { get; private set; }
		public int FuzzyMatchCount { get; private set; }
		public int ExactMatchCount { get; private set; }
		public int CountOfQueryKeywordsArePrefix { get; set; }
		public int CountOfQueryKeywordsAreFuzzy { get; set; }
		public int CountOfQueryKeywordsAreExact { get; set; }
		public int Score { get; private set; }
		public double Accuracy => (double)FuzzyMatchCount / Keywords.Length;

		public static Rank ToRank(Hotspot hotspot, Query query)
		{
			string[] keywords;
			if (string.IsNullOrEmpty(hotspot.Keywords))
			{
				keywords = ExtractKeywords(hotspot.URL);
			}
			else
			{
				keywords = hotspot.Keywords.Split(',');
			}

			var rank = new Rank()
			{
				Hotspot = hotspot,
				Keywords = keywords,
				PrefixMatchCount = CountPrefixMatches(),
				FuzzyMatchCount = CountFuzzyMatches(),
				ExactMatchCount = CountExactMatches(),
				Score = GetScore(),
			};

			rank.CountOfQueryKeywordsArePrefix = query.Keywords.Count(x => keywords.Any(y => IsPrefixOf(x, y)));
			rank.CountOfQueryKeywordsAreFuzzy = query.Keywords.Count(x => keywords.Any(y => y.Contains(x)));
			rank.CountOfQueryKeywordsAreExact = query.Keywords.Count(x => keywords.Any(y => x == y));

			return rank;

			int CountExactMatches() => keywords.Count(keyword => query.Keywords.Any(queryKeyword => keyword == queryKeyword));

			int CountPrefixMatches() => keywords.Count(keyword => query.Keywords.Any(queryKeyword => IsPrefixOf(queryKeyword, keyword)));

			int CountFuzzyMatches() => keywords.Count(keyword => query.Keywords.Any(queryKeyword => keyword.Contains(queryKeyword)));

			int GetScore()
			{
				int sum = 0;
				foreach (var queryKeyword in query.Keywords)
				{
					var candidates = keywords.Where(x => x.Contains(queryKeyword));
					if (candidates.Any())
					{
						sum += candidates.Max(x => Fuzz.Ratio(x, queryKeyword));
					}
				}

				return sum;
			}
		}

		static string[] ExtractKeywords(string url)
		{
			url = Regex.Replace(url, @"^http(s)?://", string.Empty);

			return Regex.Split(url, "[/.]").Select(x => x.Trim()).ToArray();
		}

		static bool IsPrefixOf(string x, string expected) => Regex.IsMatch(expected, $@"^{x}.*");
	}
}

