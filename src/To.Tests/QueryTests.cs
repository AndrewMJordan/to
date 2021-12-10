using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Andtech.To.Tests
{
	public class Tests
	{
		private Hotspot[] hotspots;

		[SetUp]
		public void InitializeHotspots()
		{
			hotspots = Hotspot.Read("TestFiles/hotspots.json");
		}

		[Test]
		public void SearchRoot()
		{
			var query = Query.Parse("meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://metacortex.com", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomain()
		{
			var query = Query.Parse("github meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://github.com/metacortex", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithSuffix()
		{
			var query = Query.Parse("meta cortex org");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://metacortex.org", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithPrefix()
		{
			var query = Query.Parse("oss meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://oss.metacortex.com", first.Hotspot.URL);
		}
	}
}