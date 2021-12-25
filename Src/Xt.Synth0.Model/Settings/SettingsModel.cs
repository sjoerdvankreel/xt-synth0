﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xt.Synth0.Model
{
	public sealed class SettingsModel : ICopyModel, INotifyPropertyChanged
	{
		const string DefaultOutputPath = "synth0.raw";
		public const int MaxRecentFiles = 10;

		public event EventHandler ThemeChanged;
		public event PropertyChangedEventHandler PropertyChanged;

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

		bool _useAsio;
		public bool UseAsio
		{
			get => _useAsio;
			set => Set(ref _useAsio, value);
		}

		SampleRate _sampleRate;
		public SampleRate SampleRate
		{
			get => _sampleRate;
			set => Set(ref _sampleRate, value);
		}

		BitDepth _bitDepth;
		public BitDepth BitDepth
		{
			get => _bitDepth;
			set => Set(ref _bitDepth, value);
		}

		BufferSize _bufferSize;
		public BufferSize BufferSize
		{
			get => _bufferSize;
			set => Set(ref _bufferSize, value);
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