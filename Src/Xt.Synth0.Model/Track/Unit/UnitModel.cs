using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class UnitModel : GroupModel
	{
		internal struct Native
		{
			internal int on;
			internal int amp;
			internal int oct;
			internal int note;
			internal int cent;
			internal int type;
		}

		public const int MinOctave = 0;
		public const int MaxOctave = 9;

		public static readonly string[] Notes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		public static readonly string[] Types = Enum.
			GetValues<UnitType>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo OnInfo = new ToggleInfo(
			nameof(On), "Enabled");
		static readonly ParamInfo OctInfo = new DiscreteInfo(
			nameof(Oct), "Octave", MinOctave, MaxOctave, 4);
		static readonly ParamInfo CentInfo = new DiscreteInfo(
			nameof(Cent), "Cent", -50, 49, 0);
		static readonly ParamInfo AmpInfo = new ContinuousInfo(
			nameof(Amp), "Volume", 255);
		static readonly ParamInfo NoteInfo = new EnumInfo<UnitNote>(
			nameof(Note), "Note", Notes);
		static readonly ParamInfo TypeInfo = new EnumInfo<UnitType>(
			nameof(Type), "Waveform", Types);

		public Param On { get; } = new(OnInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Type { get; } = new(TypeInfo);

		public UnitModel() : base(null) { }
		internal UnitModel(string name) : base(name) { }

		internal override Param[][] ListParamGroups() => new[] {
			new[] { On, Type },
			new[] { Amp, Oct },
			new[] { Note, Cent }
		};

		internal void ToNative(ref Native native)
		{
			native.on = On.Value;
			native.amp = Amp.Value;
			native.oct = Oct.Value;
			native.note = Note.Value;
			native.cent = Cent.Value;
			native.type = Type.Value;
		}

		internal void FromNative(ref Native native)
		{
			On.Value = native.on;
			Amp.Value = native.amp;
			Oct.Value = native.oct;
			Note.Value = native.note;
			Cent.Value = native.cent;
			Type.Value = native.type;
		}
	}
}