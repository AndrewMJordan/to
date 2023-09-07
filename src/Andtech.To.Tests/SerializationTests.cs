using Andtech.To;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Andtech.To.Tests
{

    internal class SerializationTests
	{
		private static string TestFilesRoot = "TestFiles";

		[Test]
		public void ReadJson()
		{
			var path = Path.Combine(TestFilesRoot, "single.json");
			var hotspots = Hotspot.ReadMany(path);
			Assert.AreEqual(hotspots.Length, 1);
			Assert.IsNotEmpty(hotspots[0].url);
		}

		[Test]
		public void ReadYaml()
		{
			var path = Path.Combine(TestFilesRoot, "single.yaml");
			var hotspots = Hotspot.ReadMany(path);
			Assert.AreEqual(hotspots.Length, 1);
			Assert.IsNotEmpty(hotspots[0].url);
		}
	}
}
