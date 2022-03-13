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
		internal ref struct SequencerState
		{
            internal int row;
            internal int rate;
            internal int frames;
            internal int voices;
            internal int end;
            internal int clip;
            internal int exhausted;
            internal int pad__;
            internal float* buffer;
            internal long position;
            internal SynthModel.Native* synth;
            internal SequencerModel.Native* sequencer;
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

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSequencerDSPCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SequencerState* XtsSequencerStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern PlotState* XtsPlotStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SequencerModel.Native* XtsSequencerModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SynthModel.Native* XtsSynthModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern ParamBinding.Native* XtsParamBindingCreate(int count);

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerStateDestroy(SequencerState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotStateDestroy(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerModelDestroy(SequencerModel.Native* model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelDestroy(SynthModel.Native* model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsParamBindingDestroy(ParamBinding.Native* binding);

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPRender(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerDSPRender(IntPtr dsp, SequencerState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelInit(
			ParamInfo.Native* infos, int infoCount, SyncStepModel.Native* steps, int stepCount);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSequencerDSPInit(
			IntPtr dsp, SequencerModel.Native* model, SynthModel.Native* synth, ParamBinding.Native* binding);
	}
}