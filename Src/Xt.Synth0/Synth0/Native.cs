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
		internal ref struct SeqState
		{
			internal int row, voices;
			internal int clip, exhausted;
			internal int rate, frames;
			internal int end, pad__;
			internal long pos;
			internal float* buffer;
			internal SynthModel.Native* synth;
			internal SeqModel.Native* seq;
		};

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct PlotState
		{
            internal int spec;
            internal int clip;
            internal int stereo;
			internal float* lSamples;
            internal float* rSamples;
            internal float* vSplitVals;
			internal int* hSplitVals;
			internal int bpm, pixels;
			internal float freq, rate, min, max;
			internal ushort** vSplitMarkers;
			internal ushort** hSplitMarkers;
			internal int sampleCount, hSplitCount, vSplitCount;
			internal SynthModel.Native* synth;
			IntPtr lSampleData;
            IntPtr rSampleData;
            IntPtr vSplitValData;
			IntPtr hSplitValData;
			IntPtr vSplitData;
			IntPtr hSplitData;
			IntPtr vSplitMarkerData;
			IntPtr hSplitMarkerData;
			IntPtr fftData;
			IntPtr fftScratch;
		};

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSeqDSPCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SeqState* XtsSeqStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern PlotState* XtsPlotStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SeqModel.Native* XtsSeqModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SynthModel.Native* XtsSynthModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern ParamBinding.Native* XtsParamBindingCreate();

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqStateDestroy(SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotStateDestroy(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqModelDestroy(SeqModel.Native* model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelDestroy(SynthModel.Native* model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsParamBindingDestroy(ParamBinding.Native* binding);

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPRender(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPRender(IntPtr dsp, SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelInit(
			ParamInfo.Native* infos, int infoCount, SyncStepModel.Native* steps, int stepCount);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPInit(
			IntPtr dsp, SeqModel.Native* model, SynthModel.Native* synth, ParamBinding.Native* binding);
	}
}