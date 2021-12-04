using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel : ICopyModel
	{
		public const int CurrentVersion = 1;
		public event EventHandler ParamChanged;

		public int Version { get; set; } = CurrentVersion;

		public PatternModel Pattern { get; } = new();
		public AmpModel Amp { get; } = new(nameof(Amp));
		public UnitModel Unit1 { get; } = new(nameof(Unit1));
		public UnitModel Unit2 { get; } = new(nameof(Unit2));
		public UnitModel Unit3 { get; } = new(nameof(Unit3));
		public TrackModel Track { get; } = new(nameof(Track));

		readonly SubModel[] _subModels;
		readonly List<Param> _autoParams = new();
		public IList<Param> AutoParams() => _autoParams;

		public void CopyTo(ICopyModel model)
		{
			var synth = (SynthModel)model;
			for (int s = 0; s < _subModels.Length; s++)
				_subModels[s].CopyTo(synth._subModels[s]);
		}

		public SynthModel()
		{
			PropertyChangedEventHandler handler;
			handler = (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			_subModels = new SubModel[] { Unit1, Unit2, Unit3, Amp, Track, Pattern };
			foreach (var sub in _subModels)
				foreach (var param in sub.Params())
					param.PropertyChanged += handler;
			foreach (var group in _subModels.OfType<GroupModel>().Where(m => m.Automation()))
				_autoParams.AddRange(group.Params());
		}
	}
}