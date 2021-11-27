using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class UnitModel : GroupModel<UnitModel>
	{
		public static readonly string[] Notes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		public static readonly string[] Types = Enum.
			GetValues<UnitType>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo SInfo = new ContinuousInfo(nameof(S), 255);
		static readonly ParamInfo AInfo = new LogInfo(nameof(A), 0, 1000, "ms", "s");
		static readonly ParamInfo DInfo = new LogInfo(nameof(D), 0, 3000, "ms", "s");
		static readonly ParamInfo RInfo = new LogInfo(nameof(R), 0, 10000, "ms", "s");

		static readonly ParamInfo AmpInfo = new ContinuousInfo(nameof(Amp), 255);
		static readonly ParamInfo OctInfo = new DiscreteInfo(nameof(Oct), 0, 12, 4);
		static readonly ParamInfo CentInfo = new DiscreteInfo(nameof(Cent), -50, 49, 0);
		static readonly ParamInfo NoteInfo = new EnumInfo<UnitNote>(nameof(Note), Notes);

		static readonly ParamInfo OnInfo = new ToggleInfo(nameof(On));
		static readonly ParamInfo TypeInfo = new EnumInfo<UnitType>(nameof(Type), Types);

		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param S { get; } = new(SInfo);
		public Param R { get; } = new(RInfo);
		public Param On { get; } = new(OnInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Type { get; } = new(TypeInfo);

		public override Param[][] Params() => new[] {
			new[] { On },
			new[] { Type },
			new[] { Amp, A },
			new[] { Oct, D },
			new[] { Note, S },
			new[] { Cent, R },
		};
	}
}