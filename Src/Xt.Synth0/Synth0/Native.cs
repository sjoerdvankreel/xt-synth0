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
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct XtsSequencer
        {
            float rate;
            int pad__;
            IntPtr dsp;
            internal SynthModel.Native synth;
            internal SequencerModel.Native model;
            internal ParamBinding.Native binding;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct PlotState
        {
            internal float min;
            internal float max;
            internal float rate;
            internal float frequency;

            internal int clip;
            internal int stereo;
            internal int spectrum;

            internal int bpm;
            internal int pixels;
            internal int sampleCount;
            internal int verticalCount;
            internal int horizontalCount;

            internal float* left;
            internal float* right;
            internal float* verticalValues;
            internal ushort** verticalTexts;
            internal int* horizontalValues;
            internal ushort** horizontalTexts;

            internal IntPtr leftData;
            internal IntPtr rightData;

            internal SynthModel.Native* synth;
            internal IntPtr fft;
            internal IntPtr scratch;

            internal IntPtr verticalValueData;
            internal IntPtr verticalTextData;
            internal IntPtr verticalData;

            internal IntPtr horizontalValueData;
            internal IntPtr horizontalTextData;
            internal IntPtr horizontalData;
        };

        [DllImport("XT.Synth0.DSP")] internal static extern PlotState* XtsPlotStateCreate();
        [DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPRender(PlotState* state);
        [DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotStateDestroy(PlotState* state);

        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelInit(ParamInfo.Native* @params, int count);
        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSyncStepModelInit(SyncStepModel.Native* steps, int count);

        [DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerDestroy(XtsSequencer* sequencer);
        [DllImport("XT.Synth0.DSP")] internal static extern XtsSequencer* XtsSequencerCreate(int @params, int frames, float rate);
        [DllImport("XT.Synth0.DSP")] internal static extern SequencerOutput* XtsSequencerRender(XtsSequencer* sequencer, int frames);
    }
}