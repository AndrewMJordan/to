using System.IO;
using System.Text.Json;

namespace Andtech.To
{

	public class Hotspot
    {
        public string Name { get; set; }
        public string Keywords { get; set; }
        public string URL { get; set; }
        public int Priority { get; set; }
        public string SearchURL { get; set; }
        public string Alias { get; set; }

        public static Hotspot[] Read(string path)
        {
            var content = File.ReadAllText(path);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<Hotspot[]>(content, options);
        }

        public override string ToString() => string.IsNullOrEmpty(Name) ? URL : Name;
	}
}

