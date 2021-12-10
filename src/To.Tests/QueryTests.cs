using NUnit.Framework;
using System;
using System.Linq;

namespace Andtech.To.Tests
{

	public class Tests
	{
		private Session session;

		[SetUp]
		public void InitializeHotspots()
		{
			Environment.SetEnvironmentVariable("ANDTECH_TO_PATH", "TestFiles:TestFiles/hotspots", EnvironmentVariableTarget.Process);
			session = Session.Load();
		}

		[Test]
		public void SearchMiscellaneousHotspots()
		{
			Assert.IsTrue(session.Hotspots.Any(x => x.URL == "https://youtube.com"));
		}

		[Test]
		public void SearchRoot()
		{
			var query = Query.Parse("meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://metacortex.com", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomain()
		{
			var query = Query.Parse("github meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://github.com/metacortex", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomainExtension()
		{
			var query = Query.Parse("meta cortex org");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://metacortex.org", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomainSubdomain()
		{
			var query = Query.Parse("oss meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://oss.metacortex.com", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithSimilar()
		{
			var query = Query.Parse("snip");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://github.com/andtechstudios/snip", first.Hotspot.URL);
		}
	}
}