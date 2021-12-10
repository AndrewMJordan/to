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
			var delimiter = ";";
			Environment.SetEnvironmentVariable("ANDTECH_TO_PATH", $"TestFiles{delimiter}TestFiles/hotspots", EnvironmentVariableTarget.Process);
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

			var result = ranks.First();

			Assert.AreEqual("https://metacortex.com", result.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomain()
		{
			var query = Query.Parse("github meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var result = ranks.First();

			Assert.AreEqual("https://github.com/metacortex", result.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomainExtension()
		{
			var query = Query.Parse("meta cortex org");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var result = ranks.First();

			Assert.AreEqual("https://metacortex.org", result.Hotspot.URL);
		}

		[Test]
		public void SearchWithDomainSubdomain()
		{
			var query = Query.Parse("oss meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var result = ranks.First();

			Assert.AreEqual("https://oss.metacortex.com", result.Hotspot.URL);
		}

		[Test]
		public void SearchWithSimilar()
		{
			var query = Query.Parse("snip");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var result = ranks.First();

			Assert.AreEqual("https://github.com/andtechstudios/snip", result.Hotspot.URL);
		}

		[Test]
		public void SearchForDeepMatch()
		{
			var query = Query.Parse("cypher");
			var selector = new HotspotSelector();
			var ranks = selector.Order(session.Hotspots, query);

			var result = ranks.First();

			Assert.AreEqual("https://gitlab.com/morpheus/nebuchadnezzar/zion/cypher", result.Hotspot.URL);
		}
	}
}