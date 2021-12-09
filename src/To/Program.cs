using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;

namespace Andtech.To
{

    internal class Rank
    {
        public Hotspot Hotspot { get; private set;}
        public string[] Keywords { get; private set; }
        public int MatchCount { get; private set; }

        public static Rank ToRank(Hotspot hotspot, string query)
        {
            var keywords = hotspot.Keywords?.Split(',') ?? ExtractKeywords(hotspot.URL);

            return new Rank()
			{
                Hotspot = hotspot,
                Keywords = keywords,
                MatchCount = GetKeywordMatchCount(hotspot),
			};

            int GetKeywordMatchCount(Hotspot hotspot)
            {
                var count = keywords.Count(keyword => query.Contains(keyword));

                return count;
            }

            string[] ExtractKeywords(string url)
            {
                url = url.Replace("https://", string.Empty);
                url = url.Replace(".com", string.Empty);
                return url.Split('/', StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }

	class Program
    {

        static void Main(string[] tokens)
        {
            var input = string.Join(" ", tokens);

            var hotspotMatch = Regex.Match(input, @"^(?<value>[^/?]+)");
            var suffixMatch = Regex.Match(input, @"/(?<value>.+)");
            var searchMatch = Regex.Match(input, @"\?(?<value>.+)");

            var hotspotString = hotspotMatch.Groups["value"].Value.Trim();
            var suffixString = suffixMatch.Success ? suffixMatch.Groups["value"].Value.Trim() : null;
            var searchString = searchMatch.Success ? searchMatch.Groups["value"].Value.Trim() : null;

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "to.json");
            var content = File.ReadAllText(path);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var hotspots = JsonSerializer.Deserialize<Hotspot[]>(content, options);

            var results = hotspots
                .Select(x => Rank.ToRank(x, hotspotString))
                .OrderByDescending(x => x.MatchCount)
                .ThenByDescending(x => (double)x.MatchCount / x.Keywords.Length);

            if (results.Any(x => x.MatchCount > 0))
            {
                var hotspot = results.First().Hotspot;
                string url;
                if (searchString is null || string.IsNullOrEmpty(hotspot.SearchURL))
                {
                    url = hotspot.URL;
                }
                else
                {
                    var query = searchString;
                    query = HttpUtility.UrlEncode(query);
                    url = Regex.Replace(hotspot.SearchURL, "%{query}", query);
				}

                if (suffixString != null)
				{
                    url = $"{url}/{suffixString}";
				}

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(url);
                //ShellUtility.OpenBrowser(url);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No matches");
            }
            Console.ResetColor();
        }

        static bool IsPrefixOf(string x, string full) => Regex.IsMatch(full, $@"^{x}.*");
    }
}

