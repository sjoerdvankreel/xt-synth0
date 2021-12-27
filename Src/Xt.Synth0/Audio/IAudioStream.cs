using System;

namespace Xt.Synth0
{
	internal interface IAudioStream : IDisposable
	{
		void Stop();
		void Start();
		double GetLatencyMs();
		int GetMaxBufferFrames();
	}
}