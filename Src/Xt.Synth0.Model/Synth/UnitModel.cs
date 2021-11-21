namespace Xt.Synth0.Model
{
	public class UnitModel : GroupModel<UnitModel>
	{
		static readonly ParamInfo<bool> OnInfo = ParamInfo.Of(nameof(On), false);
		static readonly ParamInfo<int> CentInfo = new(ParamType.Int, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo<int> AmpInfo = new(ParamType.Float, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo<int> OctaveInfo = new(ParamType.Int, nameof(Octave), 0, 12, 4);
		static readonly ParamInfo<int> NoteInfo = new(ParamType.Note, nameof(Note), NoteModel.C, NoteModel.B, NoteModel.C);
		static readonly ParamInfo<int> TypeInfo = new(ParamType.Type, nameof(Type), TypeModel.Sine, TypeModel.Tri, TypeModel.Sine);

		public Param<bool> On { get; } = Param.Of(OnInfo);
		public Param<int> Amp { get; } = Param.Of(AmpInfo);
		public Param<int> Note { get; } = Param.Of(NoteInfo);
		public Param<int> Cent { get; } = Param.Of(CentInfo);
		public Param<int> Type { get; } = Param.Of(TypeInfo);
		public Param<int> Octave { get; } = Param.Of(OctaveInfo);

		public override Param<bool>[] BoolParams() => new[] { On };
		public override Param<int>[] IntParams() => new[] { Type, Amp, Octave, Note, Cent };
	}
}