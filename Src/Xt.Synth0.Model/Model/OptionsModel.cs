namespace Xt.Synth0.Model
{
	public sealed class OptionsModel : ViewModel
	{
		ThemeType _theme;
		public ThemeType Theme
		{
			get => _theme;
			set => Set(ref _theme, value);
		}

		bool _useAsio;
		public bool UseAsio
		{
			get => _useAsio;
			set => Set(ref _useAsio, value);
		}

		string _deviceId;
		public string DeviceId
		{
			get => _deviceId;
			set => Set(ref _deviceId, value);
		}

		int _sampleRate;
		public int SampleRate
		{
			get => _sampleRate;
			set => Set(ref _sampleRate, value);
		}
	}
}