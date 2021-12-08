using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel : ICopyModel
	{
		public const int UnitCount = 3;
		public const int CurrentVersion = 1;

		static IEnumerable<UnitModel> MakeUnits()
		=> Enumerable.Range(0, UnitCount).Select(i => new UnitModel($"Unit {i + 1}"));

		public event EventHandler ParamChanged;

		public int Version { get; set; } = CurrentVersion;
		public PatternModel Pattern { get; } = new();
		public AmpModel Amp { get; } = new(nameof(Amp));
		public TrackModel Track { get; } = new(nameof(Track));

		[JsonIgnore]
		public IReadOnlyList<UnitModel> Units => _units.Items;
		[JsonProperty(nameof(Units))]
		readonly ModelList<UnitModel> _units = new(MakeUnits());

		readonly SubModel[] _subModels;
		readonly List<Param> _autoParams = new();
		public IList<Param> AutoParams() => _autoParams;

		public void CopyTo(ICopyModel model)
		{
			var synth = (SynthModel)model;
			if (_subModels.Length != synth._subModels.Length)
				throw new InvalidOperationException();
			for (int s = 0; s < _subModels.Length; s++)
				_subModels[s].CopyTo(synth._subModels[s]);
		}

		public SynthModel()
		{
			PropertyChangedEventHandler handler;
			handler = (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			_subModels = Units.Concat(new SubModel[] { Amp, Track, Pattern }).ToArray();
			foreach (var sub in _subModels)
				foreach (var param in sub.Params())
					param.PropertyChanged += handler;
			foreach (var group in _subModels.OfType<GroupModel>().Where(m => m.Automation()))
				_autoParams.AddRange(group.Params());
		}
	}
}