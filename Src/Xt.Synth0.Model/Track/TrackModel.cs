using System;

namespace Xt.Synth0.Model
{
	public sealed class TrackModel
	{
		public event EventHandler ParamChanged;
		public SynthModel Synth { get; } = new();
		public SeqModel Seq { get; } = new();

		public TrackModel()
		{
			Synth.ParamChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			Seq.ParamChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
		}

		public void CopyTo(TrackModel track)
		{
			Synth.CopyTo(track.Synth);
			Seq.CopyTo(track.Seq);
		}
	}
}