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
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://metacortex.com", result.URL);
		}

		[Test]
		public void SearchWithDomain()
		{
			var query = Query.Parse("github meta cortex");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://github.com/metacortex", result.URL);
		}

		[Test]
		public void SearchWithDomainExtension()
		{
			var query = Query.Parse("meta cortex org");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://metacortex.org", result.URL);
		}

		[Test]
		public void SearchWithDomainSubdomain()
		{
			var query = Query.Parse("oss meta cortex");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://oss.metacortex.com", result.URL);
		}

		[Test]
		public void SearchWithSimilar()
		{
			var query = Query.Parse("snip");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://github.com/andtechstudios/snip", result.URL);
		}

		[Test]
		public void SearchForDeepMatch()
		{
			var query = Query.Parse("cypher");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://gitlab.com/morpheus/nebuchadnezzar/zion/cypher", result.URL);
		}

		[Test]
		public void SearchByAlias()
		{
			var query = Query.Parse("gh");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://github.com", result.URL);
		}
	}
}