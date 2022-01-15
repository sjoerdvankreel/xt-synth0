using System;
using System.Runtime.InteropServices;
using System.Security;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	[SuppressUnmanagedCodeSecurity]
	internal static unsafe class Native
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct PlotInput { int rate; int pixels; IntPtr synth; }
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct PlotOutput { float freq; int rate; int bipolar; int splitCount; int sampleCount; int pad__; float* samples; int* splits; }
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct SeqState { float rate; int frames; int currentRow; int pad__; long streamPosition; float* buffer; IntPtr synth; IntPtr seq; }

		static Native() { XtsDSPInit(); }

		[DllImport("XT.Synth0.DSP")] static extern void XtsDSPInit();

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSeqModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSynthModelCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqModelDestroy(IntPtr seq);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSynthModelDestroy(IntPtr synth);

		[DllImport("XT.Synth0.DSP")] internal static extern SeqState* XtsSeqStateCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern PlotInput* XtsPlotInputCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern PlotOutput* XtsPlotOutputCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqStateDestroy(SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotInputDestroy(PlotInput* input);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotOutputDestroy(PlotOutput* output);

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsSeqDSPCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPInit(IntPtr dsp, SeqState* state);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsSeqDSPProcessBuffer(IntPtr dsp, SeqState* state);

		[DllImport("XT.Synth0.DSP")] internal static extern IntPtr XtsPlotDSPCreate();
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")] internal static extern void XtsPlotDSPRender(IntPtr dsp, PlotInput* input, PlotOutput* output);
	}
}