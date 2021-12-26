namespace Xt.Synth0.Model
{
	public sealed class AppModel
	{
		public AudioModel Audio { get; } = new();
		public TrackModel Track { get; } = new();
		public SettingsModel Settings { get; } = new();
	}
}