using Newtonsoft.Json;
using System;
using System.IO;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	static class IO
	{
		static JsonSerializerSettings MakeSettings()
		{
			var result = new JsonSerializerSettings();
			result.Formatting = Formatting.Indented;
			result.Converters.Add(new ParamConverter());
			result.Converters.Add(new PatternConverter());
			result.MissingMemberHandling = MissingMemberHandling.Error;
			return result;
		}

		static string GetAppDataFolder()
		{
			var appData = Environment.SpecialFolder.LocalApplicationData;
			var folder = Environment.GetFolderPath(appData);
			var version = typeof(Synth0).Assembly.GetName().Version.ToString();
			var result = Path.Combine(folder, nameof(Synth0), version);
			Directory.CreateDirectory(result);
			return result;
		}

		internal static void LogError(Exception error)
		{
			var file = Synth0.StartTime.ToString("yyyy-MM-dd HH.mm.ss");
			var path = Path.Combine(GetAppDataFolder(), $"{file}.log");
			using var writer = new StreamWriter(path, true);
			writer.WriteLine($"{DateTime.Now}: {error}");
		}

		internal static void Save(SynthModel model, string path)
		{
			var json = JsonConvert.SerializeObject(model, MakeSettings());
			File.WriteAllText(path, json);
		}

		internal static void Load(string path, SynthModel model)
		{
			var json = File.ReadAllText(path);
			var newModel = new SynthModel();
			JsonConvert.PopulateObject(json, newModel, MakeSettings());
			if (newModel.Version != SynthModel.CurrentVersion)
				throw new InvalidOperationException("Wrong file format version.");
			newModel.CopyTo(model);
		}
	}
}