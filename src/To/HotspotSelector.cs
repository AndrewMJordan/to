using System.Collections.Generic;
using System.Linq;

namespace Andtech.To
{

	public class HotspotSelector
	{

		public IEnumerable<Rank> Order(IEnumerable<Hotspot> hotspots, Query query)
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
