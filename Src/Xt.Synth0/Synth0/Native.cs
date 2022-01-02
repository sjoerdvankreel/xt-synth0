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
		internal static extern IntPtr XtsUnitDSPCreate();
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsUnitDSPReset(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsUnitDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern float XtsUnitDSPFrequency(IntPtr dsp, IntPtr unit);
		[DllImport("XT.Synth0.DSP")]
		internal static extern float XtsUnitDSPNext(IntPtr dsp, IntPtr global, IntPtr unit, float rate);

		[DllImport("XT.Synth0.DSP")]
		internal static extern IntPtr XtsSequencerDSPCreate();
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerDSPReset(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerDSPDestroy(IntPtr dsp);
		[DllImport("XT.Synth0.DSP")]
		internal static extern void XtsSequencerDSPDSPProcessBuffer(
			IntPtr dsp, IntPtr seq, IntPtr synth, float rate, 
			float* buffer, int frames, int* currentRow, long* streamPosition);
	}
}
