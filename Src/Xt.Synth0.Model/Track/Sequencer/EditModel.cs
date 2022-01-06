using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int keyCount, fxCount, patternCount, activePattern; }

		public Param FxCount { get; } = new(FxCountInfo);
		public Param KeyCount { get; } = new(KeyCountInfo);
		public Param PatternCount { get; } = new(PatternCountInfo);
		public Param ActivePattern { get; } = new(ActivePatternInfo);

		public string Name => "Edit";
		public IReadOnlyList<Param> Params => new[] { KeyCount, FxCount, PatternCount, ActivePattern };
		public void* Address(void* parent) => &((SequencerModel.Native*)parent)->edit;

		static readonly ParamInfo FxCountInfo = ParamInfo.Lin(p => &((Native*)p)->fxCount, "#Fx", 0, TrackConstants.MaxFxCount, 1);
		static readonly ParamInfo KeyCountInfo = ParamInfo.Lin(p => &((Native*)p)->keyCount, "#Keys", 1, TrackConstants.MaxKeyCount, 2);
		static readonly ParamInfo ActivePatternInfo = ParamInfo.Lin(p => &((Native*)p)->activePattern, "Edit", 1, TrackConstants.PatternCount, 1);
		static readonly ParamInfo PatternCountInfo = ParamInfo.Lin(p => &((Native*)p)->patternCount, "#Patterns", 1, TrackConstants.PatternCount, 1);
	}
}