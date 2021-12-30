using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class UnitModel : IGroupModel<UnitModel>
	{
		internal const int Size = 1;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			internal int on;
			internal int amp;
			internal int oct;
			internal int note;
			internal int cent;
			internal int type;
		}

		static readonly string[] Notes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		static readonly string[] Types = Enum.
			GetValues<UnitType>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo OnInfo = new ToggleInfo(
			nameof(On), "Enabled");
		static readonly ParamInfo OctInfo = new DiscreteInfo(
			nameof(Oct), "Octave", 0, 9, 4);
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

		readonly int _number;
		internal UnitModel(int number) => _number = number;

		public int NativeSize() => Size;
		public string Name() => $"Unit {_number}";

		public Param[][] ParamGroups() => new[]
		{
			new[] { On, Type },
			new[] { Amp, Oct },
			new[] { Note, Cent }
		};

		public void CopyTo(UnitModel model)
		{
			model.On.Value = On.Value;
			model.Amp.Value = Amp.Value;
			model.Oct.Value = Oct.Value;
			model.Note.Value = Note.Value;
			model.Cent.Value = Cent.Value;
			model.Type.Value = Type.Value;
		}

		public unsafe void ToNative(IntPtr native)
		{
			var p = (Native*)native;
			p->on = On.Value;
			p->amp = Amp.Value;
			p->oct = Oct.Value;
			p->note = Note.Value;
			p->cent = Cent.Value;
			p->type = Type.Value;
		}

		public unsafe void FromNative(IntPtr native)
		{
			var p = (Native*)native;
			On.Value = p->on;
			Amp.Value = p->amp;
			Oct.Value = p->oct;
			Note.Value = p->note;
			Cent.Value = p->cent;
			Type.Value = p->type;
		}

		public void RegisterParams(Action<Param> register)
		{
			register(On);
			register(Amp);
			register(Oct);
			register(Note);
			register(Cent);
			register(Type);
		}
	}
}