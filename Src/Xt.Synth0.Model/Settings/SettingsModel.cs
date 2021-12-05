using System;

namespace Xt.Synth0.Model
{
	public sealed class SettingsModel : ViewModel, ICopyModel
	{
		public event EventHandler ThemeChanged;

		bool _useAsio;
		public bool UseAsio
		{
			get => _useAsio;
			set => Set(ref _useAsio, value);
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

		SampleRate _sampleRate;
		public SampleRate SampleRate
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