using System;

namespace Xt.Synth0.Model
{
	public class ParamInfo
	{
		public static readonly string[] NoteNames = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		public int Min { get; }
		public int Max { get; }
		public int Default { get; }
		public string Name { get; }
		public ParamType Type { get; }

		internal ParamInfo(string name)
		=> (Type, Name, Min, Max, Default) = (ParamType.Toggle, name, 0, 1, 0);
		internal ParamInfo(ParamType type, string name, int min, int max, int @default)
		=> (Type, Name, Min, Max, Default) = (type, name, min, max, @default);

		public string Format(int value) => Type switch
		{
			ParamType.Int => value.ToString(),
			ParamType.Time => value.ToString(),
			ParamType.Note => NoteNames[value],
			ParamType.Toggle => value == 0 ? "Off" : "On",
			ParamType.Type => ((UnitType)value).ToString(),
			ParamType.Percent => ((int)(100.0 * (value - Min) / (Max - Min))).ToString(),
			_ => throw new InvalidOperationException()
		};
	}
}