namespace Xt.Synth0.Model
{
	public enum ThemeGroup { Settings, Units, Envs, Lfos, Plot, Global, Pattern, Control }

	public sealed class AppModel
	{
		public TrackModel Track { get; } = new();
		public StreamModel Stream { get; } = new();
		public SettingsModel Settings { get; } = new();
	}
}