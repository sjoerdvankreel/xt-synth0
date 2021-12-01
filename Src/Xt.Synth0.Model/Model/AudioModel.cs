using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public sealed class AudioModel : ViewModel
	{
		bool _isRunning;
		public bool IsRunning
		{
			get => _isRunning;
			set => Set(ref _isRunning, value);
		}

		public IList<DeviceModel> AsioDevices { get; } = new List<DeviceModel>();
		public IList<DeviceModel> WasapiDevices { get; } = new List<DeviceModel>();
	}
}