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
			internal long pos;
			internal float* buffer;
			internal IntPtr synth, seq;
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
			internal IntPtr synth;
			internal IntPtr sampleData;
			internal IntPtr splitData;
		};

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSeqDSPCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSeqModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSynthModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern SeqState* XtsSeqStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern PlotState* XtsPlotStateCreate();

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqModelDestroy(IntPtr model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelDestroy(IntPtr model);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqStateDestroy(SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotStateDestroy(PlotState* state);

		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPRender(PlotState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPRender(IntPtr dsp, SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPInit(IntPtr dsp, IntPtr model, IntPtr synth);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelInit(SynthModel.SyncStep* steps, int count);
	}
}