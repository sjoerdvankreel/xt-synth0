using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xt.Synth0.Model
{
	public enum BitDepth { Depth16, Depth24, Depth32 }
	public enum ThemeType { Generic, Red, Green, Blue }
	public enum SampleRate { Rate44100, Rate48000, Rate96000, Rate192000 }
	public enum BufferSize { Size1, Size2, Size3, Size5, Size10, Size20, Size30, Size50, Size100 }

	public sealed class SettingsModel : INotifyPropertyChanged
	{
		public const int MaxRecentFiles = 10;
		const string DefaultOutputPath = "synth0.raw";

		public event PropertyChangedEventHandler PropertyChanged;

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