using System;

namespace Xt.Synth0.Model
{
	public class ParamInfo
	{
		public static readonly string[] RowNotes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		public static readonly string[] UnitNotes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		internal static ParamInfo Toggle(string name)
		=> new(ParamType.Toggle, name, 0, 1, 0);
		internal static ParamInfo Time(string name, int max)
		=> new(ParamType.Time, name, 0, max, 0);

		public int Min { get; }
		public int Max { get; }
		public int Default { get; }
		public string Name { get; }
		public ParamType Type { get; }

		internal ParamInfo(ParamType type, string name, int min, int max, int @default)
		=> (Type, Name, Min, Max, Default) = (type, name, min, max, @default);

		public string Format(int value) => Type switch
		{
			ParamType.RowNote => RowNotes[value],
			ParamType.RowAmp => value.ToString("X2"),
			_ => DoFormat(value).PadRight(3, ' ')
		};

		string DoFormat(int value) => Type switch
		{
			ParamType.Int => value.ToString(),
			ParamType.Time => FormatTime(value),
			ParamType.UnitNote => UnitNotes[value],
			ParamType.Toggle => value == 0 ? "Off" : "On",
			ParamType.Type => ((UnitType)value).ToString(),
			ParamType.Percent => ((int)(100.0 * (value - Min) / (Max - Min))).ToString(),
			_ => throw new InvalidOperationException()
		};

		string FormatTime(double value)
		{
			var ms = (1.0 - Math.Log(Max + 1.0 - value, Max + 1.0))*MaxTimeMs;
			if (ms < 1000) return $"{(int)ms}ms";
			return $"{(ms / 1000).ToString("0.##")}s";

			var v0 = 1 - Math.Log((Max+1 - 0), Max+1);
			var v1 = 1 - Math.Log((Max + 1 - 1 ), Max + 1);
			var v10 = 1 - Math.Log((Max + 1 - 10) , Max + 1);
			var v100 = 1 - Math.Log((Max + 1 - 100) , Max + 1);
			var v254 = 1 - Math.Log((Max + 1 - 254) , Max + 1);
			var v255 = 1 - Math.Log((Max + 1 - 255) , Max + 1);
			return "";
		}
	}
}