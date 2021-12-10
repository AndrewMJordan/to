using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Andtech.To.Tests
{

	public class Tests
	{
		private Hotspot[] hotspots;

		[SetUp]
		public void InitializeHotspots()
		{
			Environment.SetEnvironmentVariable("ANDTECH_TO_PATH", "TestFiles:TestFiles/hotspots", EnvironmentVariableTarget.Process);
			var toPath = Environment.GetEnvironmentVariable("ANDTECH_TO_PATH", EnvironmentVariableTarget.Process);
			var directories = toPath.Split(':', StringSplitOptions.RemoveEmptyEntries);
			var hotspots = new List<Hotspot>();
			foreach (var directory in directories)
			{
				var files = Directory.EnumerateFiles(directory, "*.json", SearchOption.AllDirectories);
				hotspots.AddRange(files.SelectMany(Hotspot.Read));
			}
			this.hotspots = hotspots.ToArray();
		}

		[Test]
		public void SearchMiscellaneousHotspots()
		{
			Assert.IsTrue(hotspots.Any(x => x.URL == "https://youtube.com"));
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
		public void SearchWithExtension()
		{
			var query = Query.Parse("meta cortex org");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://metacortex.org", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithSubdomain()
		{
			var query = Query.Parse("oss meta cortex");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://oss.metacortex.com", first.Hotspot.URL);
		}

		[Test]
		public void SearchWithSimilar()
		{
			var query = Query.Parse("snip");
			var selector = new HotspotSelector();
			var ranks = selector.Order(hotspots, query);

			var first = ranks.First();

			Assert.AreEqual("https://github.com/andtechstudios/snip", first.Hotspot.URL);
		}
	}
}