using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	static class IO
	{
		static string GetSettingsPath()
		=> Path.Combine(GetAppDataFolder(), "settings.json");

		static JsonSerializerSettings MakeSettings()
		{
			var result = new JsonSerializerSettings();
			result.Formatting = Formatting.Indented;
			result.MissingMemberHandling = MissingMemberHandling.Error;
			var enumConverter = new StringEnumConverter();
			enumConverter.AllowIntegerValues = false;
			result.Converters.Add(enumConverter);
			return result;
		}

		static JsonSerializerSettings MakeFileSettings()
		{
			var result = MakeSettings();
			result.Converters.Add(new ParamConverter());
			result.Converters.Add(new PatternConverter());
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

		internal static void LoadSetting(SettingsModel model)
		{
			var path = GetSettingsPath();
			if (!File.Exists(path)) return;
			var json = File.ReadAllText(path);
			var newModel = new SettingsModel();
			JsonConvert.PopulateObject(json, newModel, MakeFileSettings());
			newModel.CopyTo(model);
		}

		internal static void SaveSettings(SettingsModel model)
		{
			var json = JsonConvert.SerializeObject(model, MakeSettings());
			File.WriteAllText(GetSettingsPath(), json);
		}

		internal static void SaveFile(SynthModel model, string path)
		{
			var json = JsonConvert.SerializeObject(model, MakeFileSettings());
			File.WriteAllText(path, json);
		}

		internal static void LoadFile(string path, SynthModel model)
		{
			var json = File.ReadAllText(path);
			var newModel = new SynthModel();
			JsonConvert.PopulateObject(json, newModel, MakeFileSettings());
			if (newModel.Version != SynthModel.CurrentVersion)
				throw new InvalidOperationException("Wrong file format version.");
			newModel.CopyTo(model);
		}

		internal static void LogError(DateTime startTime, string message, string trace)
		{
			var file = startTime.ToString("yyyy-MM-dd HH.mm.ss");
			var path = Path.Combine(GetAppDataFolder(), $"{file}.log");
			using var writer = new StreamWriter(path, true);
			writer.WriteLine($"{DateTime.Now}: {message}: {trace}");
		}
	}
}