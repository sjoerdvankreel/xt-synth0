namespace Xt.Synth0
{
	internal class DeviceStream : IAudioStream
	{
		XtDevice _device;
		XtStream _stream;
		internal DeviceStream(XtDevice device, XtStream stream)
		=> (_device, _stream) = (device, stream);

		public void Start() => _stream?.Start();
		public void Stop() => _stream?.Stop();

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