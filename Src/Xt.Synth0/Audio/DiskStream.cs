using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Xt.Synth0
{
	internal class DiskStream : IAudioStream
	{
		bool _running;
		bool _disposed;

		readonly IntPtr _buffer;
		readonly XtFormat _format;
		readonly FileStream _stream;
		readonly AudioEngine _engine;
		readonly int _bufferSizeBytes;
		readonly int _bufferSizeFrames;
		readonly object _lock = new object();

		public double GetLatencyMs() => 0.0;
		public int GetMaxBufferFrames() => _bufferSizeFrames;

		internal DiskStream(AudioEngine engine, in XtFormat format, int bufferSizeMs, string outputPath)
		{
			var attrs = XtAudio.GetSampleAttributes(format.mix.sample);
			_format = format;
			_engine = engine;
			_bufferSizeFrames = (int)Math.Ceiling(bufferSizeMs / 1000.0 * format.mix.rate);
			_bufferSizeBytes = _bufferSizeFrames * attrs.size * 2;
			_buffer = Marshal.AllocHGlobal(_bufferSizeBytes);
			_stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
			new Thread(Run).Start();
		}

		public void Start()
		{
			lock (_lock)
			{
				_running = true;
				Monitor.Pulse(_lock);
			}
		}

		public void Stop()
		{
			lock (_lock)
			{
				_running = false;
				Monitor.Pulse(_lock);
			}
		}

		public void Dispose()
		{
			lock (_lock)
			{
				_running = false;
				_disposed = true;
				Monitor.Pulse(_lock);
			}
		}

		void Run()
		{
			try
			{
				Write();
			}
			catch
			{
				_engine.OnRunning(false, 1);
			}
			finally
			{
				_stream.Flush();
				_stream.Dispose();
				Marshal.FreeHGlobal(_buffer);
			}
		}

		unsafe void Write()
		{
			while (true)
			{
				lock(_lock)
				{
					while (!_disposed && !_running)
						Monitor.Wait(_lock);
					if (_disposed) return;
				}
				_engine.OnRunning(true, 0);
				while (true)
				{
					lock (_lock)
					{
						if (_disposed) break;
						if (!_running) break;
					}
					var buffer = new XtBuffer();
					buffer.output = _buffer;
					buffer.frames = _bufferSizeFrames;
					_engine.OnBuffer(in buffer, _format);
					_stream.Write(new ReadOnlySpan<byte>(_buffer.ToPointer(), _bufferSizeBytes));
				}
				_engine.OnRunning(false, 0);
			}
		}
	}
}