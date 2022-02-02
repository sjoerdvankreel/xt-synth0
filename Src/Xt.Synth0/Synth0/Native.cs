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
			internal float freq, rate;
			internal int bpm, pixels;
			internal int clip, bipolar;
			internal int sampleCount, splitCount;
			internal float* samples;
			internal int* splits;
			internal SynthModel.Native* synth;
			internal IntPtr sampleData, specScratch, splitData, fftData, fftScratch;
		};

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSeqDSPCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SeqState* XtsSeqStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern PlotState* XtsPlotStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SeqModel.Native* XtsSeqModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SynthModel.Native* XtsSynthModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SynthModel.Native.VoiceBinding* XtsVoiceBindingCreate();

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqStateDestroy(SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotStateDestroy(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqModelDestroy(SeqModel.Native* model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelDestroy(SynthModel.Native* model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsVoiceBindingDestroy(SynthModel.Native.VoiceBinding* binding);

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPRender(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPRender(IntPtr dsp, SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelInit(
			SynthModel.Native.ParamInfo* infos, int infoCount, SynthModel.Native.SyncStep* steps, int stepCount);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPInit(
			IntPtr dsp, SeqModel.Native* model, SynthModel.Native* synth, SynthModel.Native.VoiceBinding* binding);
	}
}