using System;

namespace Xt.Synth0.Model
{
	public sealed class OptionsModel : ViewModel
	{
		public static readonly int[] SampleRates = new[] { 44100, 48000, 96000 };

		public event EventHandler ThemeChanged;

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

		int _sampleRate = SampleRates[0];
		public int SampleRate
		{
			get => _sampleRate;
			set => Set(ref _sampleRate, value);
		}

		ThemeType _theme;
		public ThemeType Theme
		{
			get => _theme;
			set
			{
				Set(ref _theme, value);
				ThemeChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}
}