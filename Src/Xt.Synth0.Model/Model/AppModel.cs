namespace Xt.Synth0.Model
{
	public sealed class AppModel
	{
		public SynthModel Synth { get; } = new();
		public AudioModel Audio { get; } = new();
		public SettingsModel Settings { get; } = new();
	}
}