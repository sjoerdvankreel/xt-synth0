using System;

namespace Xt.Synth0.Model
{
	public sealed class SettingsModel : ViewModel, ICopyModel
	{
		public static readonly int[] SampleRates = new[] { 44100, 48000, 96000 };

		public event EventHandler ThemeChanged;

		bool _useAsio;
		public bool UseAsio
		{
			get => _useAsio;
			set => Set(ref _useAsio, value);
		}

		int _sampleRate = SampleRates[0];
		public int SampleRate
		{
			get => _sampleRate;
			set => Set(ref _sampleRate, value);
		}

		string _asioDeviceId;
		public string AsioDeviceId
		{
			get => _asioDeviceId;
			set => Set(ref _asioDeviceId, value);
		}

		string _wasapiDeviceId;
		public string WasapiDeviceId
		{
			get => _wasapiDeviceId;
			set => Set(ref _wasapiDeviceId, value);
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

		public void CopyTo(ICopyModel model)
		{
			var settings = (SettingsModel)model;
			settings.Theme = Theme;
			settings.UseAsio = UseAsio;
			settings.SampleRate = SampleRate;
			settings.AsioDeviceId = AsioDeviceId;
			settings.WasapiDeviceId = WasapiDeviceId;
		}
	}
}