using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xt.Synth0.Model
{
	public enum BitDepth { Depth16, Depth24, Depth32 }
	public enum ThemeType { Generic, Themed, Grouped }
	public enum SampleRate { Rate44100, Rate48000, Rate96000, Rate192000 }
	public enum BufferSize { Size1, Size2, Size3, Size5, Size10, Size20, Size30, Size50, Size100 }

	public sealed class SettingsModel : IThemedModel, INotifyPropertyChanged
	{
		public const int MaxRecentFiles = 10;
		const string DefaultOutputPath = "synth0.raw";

		public string Name => "Settings";
		public ThemeGroup Group => ThemeGroup.Settings;
		public event PropertyChangedEventHandler PropertyChanged;

		ThemeType _themeType;
		public ThemeType ThemeType
		{
			get => _themeType;
			set => Set(ref _themeType, value);
		}

		string _themeColor = "#40FF00";
		public string ThemeColor
		{
			get => _themeColor;
			set => Set(ref _themeColor, value);
		}

		string _envelopeColor = "#40FF00";
		public string EnvelopeColor
		{
			get => _envelopeColor;
			set => Set(ref _envelopeColor, value);
		}

		string _lfoColor = "#40FF00";
		public string LfoColor
		{
			get => _lfoColor;
			set => Set(ref _lfoColor, value);
		}

		string _plotColor = "#40FF00";
		public string PlotColor
		{
			get => _plotColor;
			set => Set(ref _plotColor, value);
		}

		string _unitColor = "#40FF00";
		public string UnitColor
		{
			get => _unitColor;
			set => Set(ref _unitColor, value);
		}

		string _globalColor = "#40FF00";
		public string GlobalColor
		{
			get => _globalColor;
			set => Set(ref _globalColor, value);
		}

		string _patternColor = "#40FF00";
		public string PatternColor
		{
			get => _patternColor;
			set => Set(ref _patternColor, value);
		}

		string _controlColor = "#40FF00";
		public string ControlColor
		{
			get => _controlColor;
			set => Set(ref _controlColor, value);
		}

		string _settingsColor = "#40FF00";
		public string SettingsColor
		{
			get => _settingsColor;
			set => Set(ref _settingsColor, value);
		}

		bool _useAsio;
		public bool UseAsio
		{
			get => _useAsio;
			set => Set(ref _useAsio, value);
		}

		BitDepth _bitDepth;
		public BitDepth BitDepth
		{
			get => _bitDepth;
			set => Set(ref _bitDepth, value);
		}

		SampleRate _sampleRate;
		public SampleRate SampleRate
		{
			get => _sampleRate;
			set => Set(ref _sampleRate, value);
		}

		BufferSize _bufferSize;
		public BufferSize BufferSize
		{
			get => _bufferSize;
			set => Set(ref _bufferSize, value);
		}

		bool _writeToDisk;
		public bool WriteToDisk
		{
			get => _writeToDisk;
			set => Set(ref _writeToDisk, value);
		}

		string _outputPath = DefaultOutputPath;
		public string OutputPath
		{
			get => _outputPath;
			set => Set(ref _outputPath, value);
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

		ObservableCollection<string> _recentFiles = new();
		public ObservableCollection<string> RecentFiles
		{
			get => _recentFiles;
			set => Set(ref _recentFiles, value);
		}

		public void CopyTo(SettingsModel settings)
		{
			settings.ThemeType = ThemeType;
			settings.ThemeColor = ThemeColor;
			settings.LfoColor = LfoColor;
			settings.PlotColor = PlotColor;
			settings.UnitColor = UnitColor;
			settings.GlobalColor = GlobalColor;
			settings.PatternColor = PatternColor;
			settings.ControlColor = ControlColor;
			settings.SettingsColor = SettingsColor;
			settings.EnvelopeColor = EnvelopeColor;

			settings.UseAsio = UseAsio;
			settings.BitDepth = BitDepth;
			settings.SampleRate = SampleRate;
			settings.OutputPath = OutputPath;
			settings.BufferSize = BufferSize;
			settings.WriteToDisk = WriteToDisk;
			settings.AsioDeviceId = AsioDeviceId;
			settings.WasapiDeviceId = WasapiDeviceId;
			settings.RecentFiles.Clear();
			foreach (var f in RecentFiles)
				settings.RecentFiles.Add(f);
		}

		public void AddRecentFile(string path)
		{
			RecentFiles.Remove(path);
			RecentFiles.Insert(0, path);
			if (RecentFiles.Count > MaxRecentFiles)
				RecentFiles.RemoveAt(RecentFiles.Count - 1);
		}

		void Set<T>(ref T field, T value, [CallerMemberName] string property = null)
		{
			if (Equals(field, value)) return;
			field = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
	}
}