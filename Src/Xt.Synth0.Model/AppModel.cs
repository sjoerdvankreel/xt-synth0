namespace Xt.Synth0.Model
{
	public enum ThemeGroup { Settings, Unit, Env, Lfo, Global, Filter, Plot, Pattern, Control }

	public sealed class AppModel
	{
		public TrackModel Track { get; } = new();
		public SettingsModel Settings { get; } = new();
		public StreamModel Stream { get; } = new(true);
	}
}