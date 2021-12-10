using FuzzySharp;
using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.To
{

	public class Rank
	{
		public Hotspot Hotspot { get; private set; }
		public string[] Keywords { get; private set; }
		public int FuzzyMatchCount { get; private set; }
		public int ExactMatchCount { get; private set; }
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
				FuzzyMatchCount = CountFuzzyMatches(),
				ExactMatchCount = CountExactMatches(),
				Score = GetScore(),
			};

			return rank;

			int CountExactMatches() => keywords.Count(keyword => query.Keywords.Any(queryKeyword => keyword == queryKeyword));

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
			url = Regex.Replace(url, @"http(s)?://(www)?", string.Empty);

			return url.Split("/").Select(x => x.Trim()).ToArray();
		}
	}
}

