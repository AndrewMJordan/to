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
			Environment.SetEnvironmentVariable("TOPATH", $"TestFiles", EnvironmentVariableTarget.Process);
			session = Session.Load();
		}

		[Test]
		public void SearchMiscellaneousHotspots()
		{
			Assert.IsTrue(session.Hotspots.Any(x => x.url == "https://youtube.com"));
		}

		[Test]
		public void SearchRoot()
		{
			var query = Query.Parse("meta cortex");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://metacortex.com", result.url);
		}

		[Test]
		public void SearchWithDomain()
		{
			var query = Query.Parse("github meta cortex");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://github.com/metacortex", result.url);
		}

		[Test]
		public void SearchWithDomainExtension()
		{
			var query = Query.Parse("meta cortex org");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://metacortex.org", result.url);
		}

		[Test]
		public void SearchWithDomainSubdomain()
		{
			var query = Query.Parse("oss meta cortex");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://oss.metacortex.com", result.url);
		}

		[Test]
		public void SearchWithSimilar()
		{
			var query = Query.Parse("snip");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://github.com/andtechstudios/snip", result.url);
		}

		[Test]
		public void SearchForDeepMatch()
		{
			var query = Query.Parse("cypher");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://gitlab.com/morpheus/nebuchadnezzar/zion/cypher", result.url);
		}

		[Test]
		public void SearchByAlias()
		{
			var query = Query.Parse("gh");
			var selector = new HotspotSelector(session.Hotspots);
			selector.Find(query, out var result);

			Assert.AreEqual("https://github.com", result.url);
		}
	}
}