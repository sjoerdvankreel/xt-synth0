﻿namespace Xt.Synth0.Model
{
	public enum ThemeGroup { Settings, Unit, Env, Lfo, Plot, Global, Pattern, Control }

	public sealed class AppModel
	{
		public TrackModel Track { get; } = new();
		public StreamModel Stream { get; } = new();
		public SettingsModel Settings { get; } = new();
	}
}