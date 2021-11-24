namespace Xt.Synth0.Model
{
	public class UnitModel : GroupModel<UnitModel>
	{
		static readonly ParamInfo AInfo = new(ParamType.Time, nameof(A), 0, 255, 0);
		static readonly ParamInfo DInfo = new(ParamType.Time, nameof(D), 0, 255, 0);
		static readonly ParamInfo RInfo = new(ParamType.Time, nameof(R), 0, 255, 0);
		static readonly ParamInfo SInfo = new(ParamType.Percent, nameof(S), 0, 255, 255);

		static readonly ParamInfo OctInfo = new(ParamType.Int, nameof(Oct), 0, 12, 4);
		static readonly ParamInfo CentInfo = new(ParamType.Int, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo AmpInfo = new(ParamType.Percent, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo NoteInfo = new(ParamType.UnitNote,
			nameof(Note), (int)UnitNote.C, (int)UnitNote.B, (int)UnitNote.C);

		static readonly ParamInfo OnInfo = new(nameof(On));
		static readonly ParamInfo TypeInfo = new(ParamType.Type, nameof(Type),
			(int)UnitType.Sin, (int)UnitType.Tri, (int)UnitType.Sin);

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