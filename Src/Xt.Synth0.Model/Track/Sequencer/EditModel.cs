using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class EditModel : IUIParamGroupModel
    {
        public int Index => 0;
        public int Columns => 2;
        public Param Enabled => null;
        public ThemeGroup ThemeGroup => ThemeGroup.Pattern;

        public string Info => null;
        public string Name => "Edit";
        public string Id => "01ECE266-D442-4FF0-B7BE-341DED3CD55B";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SequencerModel.Native*)parent)->edit;
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>()
        {
            { Patterns, 0 }, { Bpm, 1 },
            { Rows, 2 }, { Lpb, 3 },
            { Keys, 4 }, { Edit, 5 },
            { Fxs, 6 }, { Octave, 7 } ,
            { Loop, 8 }, { Step, 9 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int fxs;
            internal int keys;
            internal int rows;
            internal int patterns;

            internal int bpm;
            internal int lpb;

            internal int step;
            internal int edit;
            internal int octave;
            internal int loop;
        }

        public Param Fxs { get; } = new(FxsInfo);
        public Param Keys { get; } = new(KeysInfo);
        public Param Rows { get; } = new(RowsInfo);
        public Param Patterns { get; } = new(PatternsInfo);
        static readonly ParamInfo FxsInfo = ParamInfo.Select(p => &((Native*)p)->fxs, 1, nameof(Fxs), "Fxs", "Effect count", 0, SequencerConfig.MaxFxs, 1);
        static readonly ParamInfo KeysInfo = ParamInfo.Select(p => &((Native*)p)->keys, 1, nameof(Keys), "Keys", "Key count", 1, SequencerConfig.MaxKeys, 2);
        static readonly ParamInfo PatternsInfo = ParamInfo.Select(p => &((Native*)p)->patterns, 1, nameof(Patterns), "Pats", "Pattern count", 1, SequencerConfig.MaxPatterns, 1);
        static readonly ParamInfo RowsInfo = ParamInfo.Select(p => &((Native*)p)->rows, 1, nameof(Rows), "Rows", "Rows per pattern", 1, SequencerConfig.MaxRows, SequencerConfig.MaxRows);

        public Param Bpm { get; } = new(BpmInfo);
        public Param Lpb { get; } = new(LpbInfo);
        static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, 0, nameof(Bpm), "BPM", "Beats per minute", 1, 255, 120);
        static readonly ParamInfo LpbInfo = ParamInfo.Select(p => &((Native*)p)->lpb, 0, nameof(Lpb), "LPB", "Lines per beat", 1, SequencerConfig.MaxLpb, 4);

        public Param Step { get; } = new(StepInfo);
        public Param Edit { get; } = new(EditInfo);
        public Param Octave { get; } = new(OctaveInfo);
        public Param Loop { get; } = new(LoopInfo);
        static readonly ParamInfo StepInfo = ParamInfo.Select(p => &((Native*)p)->step, 2, nameof(Step), "Step", "Edit step", 0, 8, 1);
        static readonly ParamInfo EditInfo = ParamInfo.Select(p => &((Native*)p)->edit, 2, nameof(Edit), "Edit", "Active pattern", 1, SequencerConfig.MaxPatterns, 1);
        static readonly ParamInfo OctaveInfo = ParamInfo.Select(p => &((Native*)p)->octave, 2, nameof(Octave), "Oct", "Octave", 0, 9, 4);
        static readonly ParamInfo LoopInfo = ParamInfo.Toggle(p => &((Native*)p)->loop, 1, nameof(Loop), "Loop", "Loop", true);
    }
}