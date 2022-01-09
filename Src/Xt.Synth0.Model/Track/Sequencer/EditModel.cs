using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int pats, rows, keys, fxs, lpb, edit; }

		public Param Fxs { get; } = new(FxsInfo);
		public Param Lpb { get; } = new(LpbInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Rows { get; } = new(RowsInfo);

		public string Name => "Edit";
		public IReadOnlyList<Param> Params => new[] { Pats, Rows, Keys, Fxs, Lpb, Edit };
		public void* Address(void* parent) => &((SequencerModel.Native*)parent)->edit;

		static readonly ParamInfo LpbInfo = ParamInfo.Lin(p => &((Native*)p)->lpb, "LPB", 1, TrackConstants.MaxLpb, 4);
		static readonly ParamInfo FxsInfo = ParamInfo.Lin(p => &((Native*)p)->fxs, nameof(Fxs), 0, TrackConstants.MaxFxs, 1);
		static readonly ParamInfo KeysInfo = ParamInfo.Lin(p => &((Native*)p)->keys, nameof(Keys), 1, TrackConstants.MaxKeys, 2);
		static readonly ParamInfo PatsInfo = ParamInfo.Lin(p => &((Native*)p)->pats, nameof(Pats), 1, TrackConstants.MaxPatterns, 1);
		static readonly ParamInfo EditInfo = ParamInfo.Lin(p => &((Native*)p)->edit, nameof(Edit), 1, TrackConstants.MaxPatterns, 1);
		static readonly ParamInfo RowsInfo = ParamInfo.Lin(p => &((Native*)p)->rows, nameof(Rows), 1, TrackConstants.MaxRows, TrackConstants.MaxRows);
	}
}