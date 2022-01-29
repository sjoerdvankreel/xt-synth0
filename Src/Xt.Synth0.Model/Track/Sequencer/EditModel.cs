using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native { internal int pats, rows, keys, fxs, bpm, lpb, step, oct, edit, pad__; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Fxs { get; } = new(FxsInfo);
		public Param Lpb { get; } = new(LpbInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Step { get; } = new(StepInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Rows { get; } = new(RowsInfo);

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
			{ Pats, 0 }, { Rows, 1 },
			{ Keys, 2 }, { Fxs, 3 },
			{ Bpm, 4 }, { Lpb, 5 },
			{ Step, 6 }, {Oct, 7} ,
			{ Edit, 8 }
		};

		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, nameof(Oct), nameof(Oct), "Octave", 0, 9, 4);
		static readonly ParamInfo StepInfo = ParamInfo.Select(p => &((Native*)p)->step, nameof(Step), nameof(Step), "Edit step", 1, 8, 1);
		static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, nameof(Bpm), "BPM", "Beats per minute", 1, 255, 120);
		static readonly ParamInfo LpbInfo = ParamInfo.Select(p => &((Native*)p)->lpb, nameof(Lpb), "LPB", "Lines per beat", 1, Model.MaxLpb, 4);
		static readonly ParamInfo FxsInfo = ParamInfo.Select(p => &((Native*)p)->fxs, nameof(Fxs), nameof(Fxs), "Effect count", 0, Model.MaxFxs, 1);
		static readonly ParamInfo KeysInfo = ParamInfo.Select(p => &((Native*)p)->keys, nameof(Keys), nameof(Keys), "Key count", 1, Model.MaxKeys, 2);
		static readonly ParamInfo PatsInfo = ParamInfo.Select(p => &((Native*)p)->pats, nameof(Pats), nameof(Pats), "Pattern count", 1, Model.MaxPatterns, 1);
		static readonly ParamInfo EditInfo = ParamInfo.Select(p => &((Native*)p)->edit, nameof(Edit), nameof(Edit), "Active pattern", 1, Model.MaxPatterns, 1);
		static readonly ParamInfo RowsInfo = ParamInfo.Select(p => &((Native*)p)->rows, nameof(Rows), nameof(Rows), "Rows per pattern", 1, Model.MaxRows, Model.MaxRows);
	}
}