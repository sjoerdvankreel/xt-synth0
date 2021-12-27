namespace Xt.Synth0
{
	internal class DeviceStream : IAudioStream
	{
		XtDevice _device;
		XtStream _stream;
		internal DeviceStream(XtDevice device, XtStream stream)
		=> (_device, _stream) = (device, stream);

		public void Stop() => _stream?.Stop();
		public void Start() => _stream?.Start();
		public int GetMaxBufferFrames() => _stream.GetFrames();
		public double GetLatencyMs() => _stream.GetLatency().output;

		public void Dispose()
		{
			_stream?.Stop();
			_stream?.Dispose();
			_device?.Dispose();
			_stream = null;
			_device = null;
		}
	}
}