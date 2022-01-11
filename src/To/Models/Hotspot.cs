using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Andtech.To
{

	public class Hotspot
	{
		public string name { get; set; }
		public string keywords { get; set; }
		public string url { get; set; }
		public int priority { get; set; }
		public string searchURL { get; set; }
		public string alias { get; set; }

        public static Hotspot[] ReadMany(string path)
		{
			var content = File.ReadAllText(path);
			var extension = Path.GetExtension(path);
			switch (extension)
			{
				case ".yaml":
				case ".yml":
					return ParseYaml(content);
				case ".json":
					return ParseJson(content);
				default:
					throw new ArgumentException($"File type {extension} not supported");
			}
		}

		public static Hotspot[] ParseYaml(string content)
        {
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();

			return deserializer.Deserialize<List<Hotspot>>(content).ToArray();
		}

		public static Hotspot[] ParseJson(string content)
		{
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			return JsonSerializer.Deserialize<Hotspot[]>(content, options);
		}

		public override string ToString() => string.IsNullOrEmpty(name) ? url : name;
	}
}

