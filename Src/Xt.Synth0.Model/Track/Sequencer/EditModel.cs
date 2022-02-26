using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native { internal int pats, rows, keys, fxs, bpm, lpb, step, oct, edit, loop; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Fxs { get; } = new(FxsInfo);
		public Param Lpb { get; } = new(LpbInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Step { get; } = new(StepInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Rows { get; } = new(RowsInfo);
		public Param Loop { get; } = new(LoopInfo);

		public int Index => 0;
		public int Columns => 2;
		public Param Enabled => null;
		public string Name => "Edit";
		public ThemeGroup ThemeGroup => ThemeGroup.Pattern;
		public string Id => "01ECE266-D442-4FF0-B7BE-341DED3CD55B";
		public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
		public void* Address(void* parent) => &((SeqModel.Native*)parent)->edit;
		public IDictionary<Param, int> Layout => new Dictionary<Param, int>()
		{
            { Pats, 0 }, { Bpm, 1 },
            { Rows, 2 }, { Lpb, 3 },
            { Keys, 4 }, { Edit, 5 },
            { Fxs, 6 }, { Oct, 7 } ,
            { Loop, 8 }, { Step, 9 }
		};

		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, 2, nameof(Oct), nameof(Oct), "Octave", 0, 9, 4);
		static readonly ParamInfo LoopInfo = ParamInfo.Toggle(p => &((Native*)p)->loop, 1, nameof(Loop), nameof(Loop), nameof(Loop), true);
		static readonly ParamInfo StepInfo = ParamInfo.Select(p => &((Native*)p)->step, 2, nameof(Step), nameof(Step), "Edit step", 0, 8, 1);
		static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, 0, nameof(Bpm), "BPM", "Beats per minute", 1, 255, 120);
		static readonly ParamInfo LpbInfo = ParamInfo.Select(p => &((Native*)p)->lpb, 0, nameof(Lpb), "LPB", "Lines per beat", 1, Model.MaxLpb, 4);
		static readonly ParamInfo FxsInfo = ParamInfo.Select(p => &((Native*)p)->fxs, 1, nameof(Fxs), nameof(Fxs), "Effect count", 0, Model.MaxFxs, 1);
		static readonly ParamInfo KeysInfo = ParamInfo.Select(p => &((Native*)p)->keys, 1, nameof(Keys), nameof(Keys), "Key count", 1, Model.MaxKeys, 2);
		static readonly ParamInfo PatsInfo = ParamInfo.Select(p => &((Native*)p)->pats, 1, nameof(Pats), nameof(Pats), "Pattern count", 1, Model.MaxPatterns, 1);
		static readonly ParamInfo EditInfo = ParamInfo.Select(p => &((Native*)p)->edit, 2, nameof(Edit), nameof(Edit), "Active pattern", 1, Model.MaxPatterns, 1);
		static readonly ParamInfo RowsInfo = ParamInfo.Select(p => &((Native*)p)->rows, 1, nameof(Rows), nameof(Rows), "Rows per pattern", 1, Model.MaxRows, Model.MaxRows);
	}
}