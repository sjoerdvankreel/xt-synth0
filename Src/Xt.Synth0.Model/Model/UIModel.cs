﻿using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Xt.Synth0.Model
{
	public class UIModel : INotifyPropertyChanged
	{
		public event EventHandler StopRequest;
		public event EventHandler StartRequest;

		public event EventHandler NewRequest;
		public event EventHandler OpenRequest;
		public event EventHandler SaveRequest;
		public event EventHandler SaveAsRequest;

		public event PropertyChangedEventHandler PropertyChanged;

		ThemeType _theme;
		public ThemeType Theme
		{
			get => _theme;
			set => Set(ref _theme, value);
		}

		bool _isRunning;
		public bool IsRunning
		{
			get => _isRunning;
			set => Set(ref _isRunning, value);
		}

		public void RequestStop() => StopRequest?.Invoke(this, EventArgs.Empty);
		public void RequestStart() => StartRequest?.Invoke(this, EventArgs.Empty);

		public void RequestNew() => NewRequest?.Invoke(this, EventArgs.Empty);
		public void RequestOpen() => OpenRequest?.Invoke(this, EventArgs.Empty);
		public void RequestSave() => SaveRequest?.Invoke(this, EventArgs.Empty);
		public void RequestSaveAs() => SaveAsRequest?.Invoke(this, EventArgs.Empty);

		void Set<T>(ref T field, T value, [CallerMemberName] string property = null)
		{
			field = value;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
	}
}