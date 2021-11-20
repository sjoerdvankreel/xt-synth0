using Newtonsoft.Json;
using System;
using System.IO;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	internal static class IO
	{
		internal static void Load(string path, SynthModel model)
		{
			var json = File.ReadAllText(path);
			var settings = new JsonSerializerSettings();
			settings.MissingMemberHandling = MissingMemberHandling.Error;
			JsonConvert.PopulateObject(json, model, settings);
		}

		internal static void Save(SynthModel model, string path)
		{
			var json = JsonConvert.SerializeObject(model, Formatting.Indented);
			File.WriteAllText(path, json);
		}

		internal static void LogError(DateTime startTime, string error)
		{
			var file = startTime.ToString("yyyy-MM-dd HH.mm.ss");
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			var path = Path.Combine(folder, nameof(Synth0));
			Directory.CreateDirectory(path);
			path = Path.Combine(path, $"{file}.log");
			using var writer = new StreamWriter(path, true);
			writer.WriteLine($"{DateTime.Now}: {error}");
		}
	}
}