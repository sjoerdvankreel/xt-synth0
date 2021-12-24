using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel
	{
		public const int UnitCount = 3;
		public const int CurrentVersion = 1;

		static IEnumerable<UnitModel> MakeUnits()
		=> Enumerable.Range(0, UnitCount).Select(i => new UnitModel($"Unit {i + 1}"));

		public event EventHandler<ParamChangedEventArgs> ParamChanged;

		public int Version { get; set; } = CurrentVersion;
		public PatternModel Pattern { get; } = new();
		public AmpModel Amp { get; } = new(nameof(Amp));
		public TrackModel Track { get; } = new(nameof(Track));
		public GlobalModel Global { get; } = new(nameof(Global));

		[JsonIgnore]
		public IReadOnlyList<UnitModel> Units => _units.Items;
		[JsonProperty(nameof(Units))]
		readonly ModelList<UnitModel> _units = new(MakeUnits());

		readonly SubModel[] _subModels;
		readonly List<Param> _params = new();
		readonly List<AutoParam> _autoParams = new();
		public IReadOnlyList<Param> Params() => _params;
		public IReadOnlyList<AutoParam> AutoParams() => _autoParams;

		public AutoParam AutoParam(Param param)
		=> AutoParams().SingleOrDefault(a => a.Param == param);

		public void CopyTo(SynthModel model, bool automationOnly)
		{
			if (_subModels.Length != model._subModels.Length)
				throw new InvalidOperationException();
			for (int s = 0; s < _subModels.Length; s++)
				if (!automationOnly || (_subModels[s] as GroupModel)?.Automation() == true)
					_subModels[s].CopyTo(model._subModels[s]);
		}

		public SynthModel()
		{
			Units[0].On.Value = 1;
			_subModels = Units.Concat(new SubModel[] { Amp, Global, Track, Pattern }).ToArray();
			_params.AddRange(_subModels.SelectMany(m => m.Params()));
			for (int i = 0; i < _params.Count; i++)
			{
				int iLocal = i;
				_params[i].PropertyChanged += (s, e) => ParamChanged?.Invoke(
					this, new ParamChangedEventArgs(iLocal, _params[iLocal].Value));
			}
			int index = 1;
			foreach (var group in _subModels.OfType<GroupModel>().Where(m => m.Automation()))
				_autoParams.AddRange(group.Params().Select(p => new AutoParam(group, p, index++)));
		}
	}
}