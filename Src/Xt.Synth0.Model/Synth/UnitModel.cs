﻿namespace Xt.Synth0.Model
{
	public class UnitModel : IGroupModel
	{
		static readonly ParamInfo<int> AmpInfo = new(ParamType.Float, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo<int> NoteInfo = new(ParamType.Note, nameof(Note), 0, 11, 0);
		static readonly ParamInfo<int> CentInfo = new(ParamType.Int, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo<int> OctaveInfo = new(ParamType.Int, nameof(Octave), 0, 12, 4);
		static readonly ParamInfo<bool> OnInfo = ParamInfo.Of(nameof(On), false);

		public Param<bool> On { get; } = Param.Of(OnInfo);
		public Param<int> Amp { get; } = Param.Of(AmpInfo);
		public Param<int> Note { get; } = Param.Of(NoteInfo);
		public Param<int> Cent { get; } = Param.Of(CentInfo);
		public Param<int> Octave { get; } = Param.Of(OctaveInfo);

		public override Param<bool>[] BoolParams() => new[] { On };
		public override Param<int>[] IntParams() => new[] { Amp, Octave, Note, Cent };

		internal void CopyTo(UnitModel model)
		{
			model.On.Value = On.Value;
			model.Amp.Value = Amp.Value;
			model.Note.Value = Note.Value;
			model.Cent.Value = Cent.Value;
			model.Octave.Value = Octave.Value;
		}
	}
}