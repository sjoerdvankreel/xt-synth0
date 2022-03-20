using MessagePack;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xt.Synth0.Model
{
	public enum DeviceType { DSound, Wasapi, Asio }
	public enum BitDepth { Depth16, Depth24, Depth32 }
	public enum SampleRate { Rate44100, Rate48000, Rate96000, Rate192000 }
	public enum BufferSize { Size1, Size2, Size3, Size5, Size10, Size20, Size30, Size50, Size100 }

	[MessagePackObject]
	public sealed class SettingsModel : IUIModel, INotifyPropertyChanged
	{
		public const int MaxRecentFiles = 10;
		const string DefaultOutputPath = "synth0.raw";
		public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreMember]
        public string Info => null;
        [IgnoreMember]
		public string Name => "Settings";
        [IgnoreMember]
		public ThemeGroup ThemeGroup => ThemeGroup.Settings;

		string _envColor = "#40E0C0";
		[Key(nameof(EnvColor))]
		public string EnvColor
		{
			get => _envColor;
			set => Set(ref _envColor, value);
		}

		string _lfoColor = "#FF8040";
		[Key(nameof(LfoColor))]
		public string LfoColor
		{
			get => _lfoColor;
			set => Set(ref _lfoColor, value);
		}

		string _plotColor = "#FF4040";
		[Key(nameof(PlotColor))]
		public string PlotColor
		{
			get => _plotColor;
			set => Set(ref _plotColor, value);
		}

		string _unitColor = "#FFC080";
		[Key(nameof(UnitColor))]
		public string UnitColor
		{
			get => _unitColor;
			set => Set(ref _unitColor, value);
		}

		string _filterColor = "#40E080";
		[Key(nameof(FilterColor))]
		public string FilterColor
		{
			get => _filterColor;
			set => Set(ref _filterColor, value);
		}

		string _ampColor = "#FF8000";
		[Key(nameof(AmpColor))]
		public string AmpColor
		{
			get => _ampColor;
			set => Set(ref _ampColor, value);
		}

		string _settingsColor = "#E0E0E0";
		[Key(nameof(SettingsColor))]
		public string SettingsColor
		{
			get => _settingsColor;
			set => Set(ref _settingsColor, value);
		}

		string _patternColor = "#00C0FF";
		[Key(nameof(PatternColor))]
		public string PatternColor
		{
			get => _patternColor;
			set => Set(ref _patternColor, value);
		}

		string _controlColor = "#A0C0FF";
		[Key(nameof(ControlColor))]
		public string ControlColor
		{
			get => _controlColor;
			set => Set(ref _controlColor, value);
		}

		BitDepth _bitDepth;
		[Key(nameof(BitDepth))]
		public BitDepth BitDepth
		{
			get => _bitDepth;
			set => Set(ref _bitDepth, value);
		}

		SampleRate _sampleRate;
		[Key(nameof(SampleRate))]
		public SampleRate SampleRate
		{
			get => _sampleRate;
			set => Set(ref _sampleRate, value);
		}

		DeviceType _deviceType;
		[Key(nameof(DeviceType))]
		public DeviceType DeviceType
		{
			get => _deviceType;
			set => Set(ref _deviceType, value);
		}

		BufferSize _bufferSize;
		[Key(nameof(BufferSize))]
		public BufferSize BufferSize
		{
			get => _bufferSize;
			set => Set(ref _bufferSize, value);
		}

		bool _writeToDisk;
		[Key(nameof(WriteToDisk))]
		public bool WriteToDisk
		{
			get => _writeToDisk;
			set => Set(ref _writeToDisk, value);
		}

		string _outputPath = DefaultOutputPath;
		[Key(nameof(OutputPath))]
		public string OutputPath
		{
			get => _outputPath;
			set => Set(ref _outputPath, value);
		}

		string _asioDeviceId;
		[Key(nameof(AsioDeviceId))]
		public string AsioDeviceId
		{
			get => _asioDeviceId;
			set => Set(ref _asioDeviceId, value);
		}

		string _dSoundDeviceId;
		[Key(nameof(DSoundDeviceId))]
		public string DSoundDeviceId
		{
			get => _dSoundDeviceId;
			set => Set(ref _dSoundDeviceId, value);
		}

		string _wasapiDeviceId;
		[Key(nameof(WasapiDeviceId))]
		public string WasapiDeviceId
		{
			get => _wasapiDeviceId;
			set => Set(ref _wasapiDeviceId, value);
		}

		ObservableCollection<string> _recentFiles = new();
		[Key(nameof(RecentFiles))]
		public ObservableCollection<string> RecentFiles
		{
			get => _recentFiles;
			set => Set(ref _recentFiles, value);
		}

		public void CopyTo(SettingsModel settings)
		{
			settings.LfoColor = LfoColor;
            settings.EnvColor = EnvColor;
            settings.AmpColor = AmpColor;
            settings.PlotColor = PlotColor;
			settings.UnitColor = UnitColor;
			settings.FilterColor = FilterColor;
			settings.PatternColor = PatternColor;
			settings.ControlColor = ControlColor;
			settings.SettingsColor = SettingsColor;

			settings.BitDepth = BitDepth;
			settings.DeviceType = DeviceType;
			settings.SampleRate = SampleRate;
			settings.OutputPath = OutputPath;
			settings.BufferSize = BufferSize;
			settings.WriteToDisk = WriteToDisk;
			settings.AsioDeviceId = AsioDeviceId;
			settings.DSoundDeviceId = DSoundDeviceId;
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