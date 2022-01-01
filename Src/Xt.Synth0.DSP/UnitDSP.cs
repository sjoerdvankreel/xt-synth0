using System;
using System.Runtime.InteropServices;
using System.Security;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	[SuppressUnmanagedCodeSecurity]
	public class UnitDSP
	{
		const string NativeLib = "Xt.Synth0.DSP.Native.dll";

		[DllImport("kernel32")]
		static extern IntPtr LoadLibrary(string path);
		[DllImport(NativeLib)]
		static extern float xts0_unit_additive(float phase);

		static UnitDSP()
		{
			if (IntPtr.Size == 4)
				LoadLibrary($"x86/{NativeLib}");
			else
				LoadLibrary($"x64/{NativeLib}");
		}

		static readonly float[,,] FrequencyTable = MakeFrequencyTable();

	

		float _phasef = 0.0f;
		double _phased = 0.0f;
		
		internal void Reset()
		{
			_phased = 0.0;
			_phasef = 0.0f;
		}

		
	}
}