using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Xt.Synth0
{
	[SuppressUnmanagedCodeSecurity]
	internal static unsafe class Native
	{
		static Native()
		{
			XtsDSPInit();
		}

		[DllImport("XT.Synth0.DSP")]
		static extern void XtsDSPInit();

		[DllImport("XT.Synth0.DSP")]
		internal static extern IntPtr XtsSynthModelCreate();
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSynthModelDestroy(IntPtr synth);
		[DllImport("XT.Synth0.DSP")]
		internal static extern IntPtr XtsSequencerModelCreate();
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerModelDestroy(IntPtr seq);

		[DllImport("XT.Synth0.DSP")]
		internal static extern IntPtr XtsSequencerDSPCreate();
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerDSPReset(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerDSPProcessBuffer(
			IntPtr dsp, IntPtr seq, IntPtr synth, float rate,
			float* buffer, int frames, int* currentRow, long* streamPosition);

		[DllImport("XT.Synth0.DSP")]
		internal static extern IntPtr XtsPlotDSPCreate();
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsPlotDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsPlotDSPRender(
		  IntPtr dsp, IntPtr synth, int pixels, int* rate, int* bipolar,
		    float* frequency, float** samples, int* sampleCount, int** splits, int* splitCount);
	}
}
