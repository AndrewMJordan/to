using System.Linq;
using System.Text.RegularExpressions;

namespace Andtech.To
{
	public struct Query
	{
		public string[] Keywords { get; set; }
		public string Path { get; set; }
		public string Search { get; set; }

		public static Query Parse(string input)
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
	}
}

