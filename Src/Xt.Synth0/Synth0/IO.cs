using MessagePack;
using System;
using System.IO;
using System.Linq;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	static class IO
	{
		static string GetSettingsPath() 
		=> Path.Combine(GetAppDataFolder(), "settings.x0s");
		static MessagePackSerializerOptions GetSerializerOptions() 
		=> MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

		static string GetAppDataFolder()
		{
			var appData = Environment.SpecialFolder.LocalApplicationData;
			var folder = Environment.GetFolderPath(appData);
			var version = typeof(Synth0).Assembly.GetName().Version.ToString();
			var result = Path.Combine(folder, nameof(Synth0), version);
			Directory.CreateDirectory(result);
			return result;
		}

		internal static void SaveSettings(SettingsModel settings)
		{
			using var stream = File.Create(GetSettingsPath());
			MessagePackSerializer.Serialize(stream, settings, GetSerializerOptions());
		}

		internal static SettingsModel LoadSettings()
		{
			var path = GetSettingsPath();
			if (!File.Exists(path)) return null;
			using var stream = File.OpenRead(path);
			return MessagePackSerializer.Deserialize<SettingsModel>(stream, GetSerializerOptions());
		}

		internal static void SaveFile(TrackModel track, string path)
		{
			var ints = track.Synth.Params.Select(p => p.Value).Concat(
				track.Seq.Params.Select(p => p.Value)).ToArray();
			var bytes = ints.SelectMany(i => BitConverter.GetBytes(i)).ToArray();
			File.WriteAllBytes(path, bytes);
		}

		internal static void LoadFile(string path, TrackModel track)
		{
			var bytes = File.ReadAllBytes(path);
			var range = Enumerable.Range(0, bytes.Length / 4);
			var ints = range.Select(i => BitConverter.ToInt32(bytes, i * 4)).ToArray();
			for (int i = 0; i < track.Synth.Params.Count; i++)
				track.Synth.Params[i].Value = ints[i];
			for (int i = 0; i < track.Seq.Params.Count; i++)
				track.Seq.Params[i].Value = ints[track.Synth.Params.Count + i];
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