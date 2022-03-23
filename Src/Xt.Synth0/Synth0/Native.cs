using System;
using System.Runtime.InteropServices;
using System.Security;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
    [SuppressUnmanagedCodeSecurity]
    internal static unsafe class Native
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct SequencerOutput
        {
            internal int row;
            internal int voices;
            internal int end;
            internal int clip;
            internal int exhausted;
            internal int pad__;
            internal float* buffer;
            internal long position;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct XtsSequencer
        {
            internal ParamBinding.Native binding;
            internal IntPtr synthDsp;
            internal SynthModel.Native synthModel;
            internal IntPtr sequencerDsp;
            internal SequencerModel.Native* sequencerModel;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct PlotInput
        {
            internal float bpm;
            internal float rate;
            internal float pixels;
            internal int spectrum;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct PlotState
        {
            internal int hold;
            internal int pad__;
            internal PlotOutput.Native output;
            internal PlotResult.Native result;
            internal IntPtr data;
            internal IntPtr scratch;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct XtsPlot
        {
            internal PlotState state;
            internal SynthModel.Native model;
            internal ParamBinding.Native binding;
        }

        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelInit(ParamInfo.Native* @params, int count);
        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSyncStepModelInit(SyncStepModel.Native* steps, int count);

        [DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDestroy(XtsPlot* plot);
        [DllImport("XT.Synth0.DSP")] internal static extern XtsPlot* XtsPlotCreate(int @params);
        [DllImport("XT.Synth0.DSP")] internal static extern PlotResult.Native* XtsPlotRender(XtsPlot* plot, PlotInput* input, PlotOutput.Native** output);

        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerDestroy(XtsSequencer* sequencer);
        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerConnect(XtsSequencer* sequencer, float rate);
        [DllImport("XT.Synth0.DSP")] internal static extern XtsSequencer* XtsSequencerCreate(int @params, int frames, float rate);
        [DllImport("XT.Synth0.DSP")] internal static extern SequencerOutput* XtsSequencerRender(XtsSequencer* sequencer, int frames, AutomationAction.Native* actions, int count);
    }
}