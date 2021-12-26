using System;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class TrackModel: ICopyModel
	{
		public const int CurrentVersion = 1;
		public event EventHandler<ParamChangedEventArgs> ParamChanged;

		readonly List<Param> _params = new();
		public IReadOnlyList<Param> Params() => _params;

		public SynthModel Synth { get; } = new();
		public SequencerModel Sequencer { get; } = new();
		public int Version { get; set; } = CurrentVersion;

		public void CopyTo(ICopyModel other)
		{
			var model = (TrackModel)other;
			Synth.CopyTo(model.Synth);
			Sequencer.CopyTo(model.Sequencer);
		}

		public TrackModel()
		{
			is this needed ?
			_params.AddRange(Synth.SubModels().SelectMany(m => m.Params()));
			_params.AddRange(Sequencer.SubModels().SelectMany(m => m.Params()));
			for (int p = 0; p < _params.Count; p++)
			{
				int iLocal = p;
				_params[p].PropertyChanged += (s, e) => ParamChanged?.Invoke(
					this, new ParamChangedEventArgs(iLocal, _params[iLocal].Value));
			}
		}
	}
}