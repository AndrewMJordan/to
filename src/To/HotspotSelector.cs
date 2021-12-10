using Andtech.To;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.To
{

	public class HotspotSelector
	{

		public IEnumerable<Rank> Order(IEnumerable<Hotspot> hotspots, Query query)
		{
			return hotspots
				.Select(x => Rank.ToRank(x, query))
				.OrderByDescending(x => x.FuzzyMatchCount)
				.ThenByDescending(x => x.Accuracy)
				.ThenByDescending(x => x.Score);
		}
	}
}
