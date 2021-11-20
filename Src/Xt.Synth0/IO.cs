using Newtonsoft.Json;
using System;
using System.IO;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	internal static class IO
	{
		static JsonSerializerSettings MakeSettings()
		{
			var result = new JsonSerializerSettings();
			result.Formatting = Formatting.Indented;
			result.Converters.Add(new ParamConverter<int>());
			result.Converters.Add(new ParamConverter<bool>());
			result.MissingMemberHandling = MissingMemberHandling.Error;
			return result;
		}

		internal static void Load(string path, SynthModel model)
		{
			var json = File.ReadAllText(path);
			JsonConvert.PopulateObject(json, model, MakeSettings());
		}

		internal static void Save(SynthModel model, string path)
		{
			var json = JsonConvert.SerializeObject(model, MakeSettings());
			File.WriteAllText(path, json);
		}

		internal static void LogError(string error)
		{
			var file = Synth0.StartTime.ToString("yyyy-MM-dd HH.mm.ss");
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var path = Path.Combine(folder, nameof(Synth0));
			Directory.CreateDirectory(path);
			path = Path.Combine(path, $"{file}.log");
			using var writer = new StreamWriter(path, true);
			writer.WriteLine($"{DateTime.Now}: {error}");
		}
	}
}