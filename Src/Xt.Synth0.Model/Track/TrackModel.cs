namespace Xt.Synth0.Model
{
	public sealed class TrackModel
	{
		public SynthModel Synth { get; } = new();
		public SequencerModel Sequencer { get; } = new();
	}
}