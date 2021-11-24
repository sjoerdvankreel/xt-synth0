namespace Xt.Synth0.Model
{
	public class UnitModel : GroupModel<UnitModel>
	{
		static readonly ParamInfo OnInfo = new(nameof(On));
		static readonly ParamInfo OctInfo = new(ParamType.Int, nameof(Oct), 0, 12, 4);
		static readonly ParamInfo CentInfo = new(ParamType.Int, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo AmpInfo = new(ParamType.Float, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo NoteInfo = new(ParamType.Note, nameof(Note), NoteModel.C, NoteModel.B, NoteModel.C);
		static readonly ParamInfo TypeInfo = new(ParamType.Type, nameof(Type), TypeModel.Sine, TypeModel.Tri, TypeModel.Sine);

		public Param On { get; } = new(OnInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Type { get; } = new(TypeInfo);

		public override Param[] Params() => new[] { On, Type, Amp, Oct, Note, Cent };
	}
}