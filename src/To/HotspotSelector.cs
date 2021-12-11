using System.Collections.Generic;
using System.Linq;

namespace Andtech.To
{

	public class HotspotSelector
	{
		private readonly IEnumerable<Hotspot> hotspots;

		public HotspotSelector(IEnumerable<Hotspot> hotspots)
		{
			this.hotspots = hotspots;
		}

		public bool Find(Query query, out Hotspot best)
		{
			if (TryGetFromAlias(query, out best))
			{
				return true;
			}

			best = Order(query)
				.Where(x => x.CountOfHotspotKeywordsAreFuzzy > 0)
				.FirstOrDefault()?
				.Hotspot;

			return best != default;
		}

		public bool TryGetFromAlias(Query query, out Hotspot result)
		{
			result = hotspots
				.Where(x => !string.IsNullOrEmpty(x.Alias))
				.FirstOrDefault(x => x.Alias == query.RawKeywords);

			return result != default;
		}

		public IEnumerable<Rank> Order(Query query)
		{
			return hotspots
				.Select(x => Rank.ToRank(x, query))
				.Where(x => x.CountOfQueryKeywordsAreFuzzy > 0)
				.OrderByDescending(x => x.CountOfQueryKeywordsAreFuzzy)
				.ThenByDescending(x => x.CountOfQueryKeywordsAreExact)
				.ThenByDescending(x => x.Score);
		}
	}
}
