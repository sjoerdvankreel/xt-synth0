using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int keys, fxs, pats, edit; }

		public Param Fxs { get; } = new(FxsInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Edit { get; } = new(EditInfo);

		public string Name => "Edit";
		public IReadOnlyList<Param> Params => new[] { Keys, Fxs, Pats, Edit };
		public void* Address(void* parent) => &((SequencerModel.Native*)parent)->edit;

		static readonly ParamInfo FxsInfo = ParamInfo.Lin(p => &((Native*)p)->fxs, nameof(Fxs), 0, TrackConstants.MaxFxCount, 1);
		static readonly ParamInfo KeysInfo = ParamInfo.Lin(p => &((Native*)p)->keys, nameof(Keys), 1, TrackConstants.MaxKeyCount, 2);
		static readonly ParamInfo PatsInfo = ParamInfo.Lin(p => &((Native*)p)->pats, nameof(Pats), 1, TrackConstants.PatternCount, 1);
		static readonly ParamInfo EditInfo = ParamInfo.Lin(p => &((Native*)p)->edit, nameof(Edit), 1, TrackConstants.PatternCount, 1);
	}
}