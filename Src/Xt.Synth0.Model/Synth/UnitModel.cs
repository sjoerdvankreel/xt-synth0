using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class UnitModel : GroupModel
	{
		const string DDetail = "Decay time";
		const string ADetail = "Attack time";
		const string RDetail = "Release time";
		const string SDetail = "Sustain level";

		const string NoteDetail = "Unit note";
		const string CentDetail = "Unit cent";
		const string OctDetail = "Unit octave";
		const string AmpDetail = "Unit volume";

		const string OnDetail = "Unit enabled";
		const string TypeDetail = "Waveform type";

		public static readonly string[] Notes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		public static readonly string[] Types = Enum.
			GetValues<UnitType>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo SInfo = new ContinuousInfo(
			nameof(S), SDetail, 255);
		static readonly ParamInfo AInfo = new LogInfo(
			nameof(A), ADetail, 0, 1000, "ms", "s");
		static readonly ParamInfo DInfo = new LogInfo(
			nameof(D), DDetail, 0, 3000, "ms", "s");
		static readonly ParamInfo RInfo = new LogInfo(
			nameof(R), RDetail, 0, 10000, "ms", "s");

		static readonly ParamInfo AmpInfo = new ContinuousInfo(
			nameof(Amp), AmpDetail, 255);
		static readonly ParamInfo OctInfo = new DiscreteInfo(
			nameof(Oct), OctDetail, 0, 12, 4);
		static readonly ParamInfo CentInfo = new DiscreteInfo(
			nameof(Cent), CentDetail, -50, 49, 0);
		static readonly ParamInfo NoteInfo = new EnumInfo<UnitNote>(
			nameof(Note), NoteDetail, Notes);

		static readonly ParamInfo OnInfo = new ToggleInfo(
			nameof(On), OnDetail);
		static readonly ParamInfo TypeInfo = new EnumInfo<UnitType>(
			nameof(Type), TypeDetail, Types);

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

		internal override Param[][] ListParamGroups() => new[] {
			new[] { On },
			new[] { Type },
			new[] { Amp, A },
			new[] { Oct, D },
			new[] { Note, S },
			new[] { Cent, R },
		};
	}
}