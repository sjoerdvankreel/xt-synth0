using System;
using System.ComponentModel;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel : Model
	{
		public const int CurrentVersion = 1;
		public event EventHandler ParamChanged;

		public UnitModel Unit1 { get; } = new();
		public UnitModel Unit2 { get; } = new();
		public UnitModel Unit3 { get; } = new();
		public GlobalModel Global { get; } = new();
		public PatternModel Pattern { get; } = new();
		public int Version { get; set; } = CurrentVersion;

		readonly SubModel[] _subModels;

		public override void CopyTo(Model model)
		{
			var synth = (SynthModel)model;
			for (int s = 0; s < _subModels.Length; s++)
				_subModels[s].CopyTo(synth._subModels[s]);
		}

		public SynthModel()
		{
			PropertyChangedEventHandler handler;
			_subModels = new SubModel[] { Unit1, Unit2, Unit3, Global, Pattern };
			handler = (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			foreach (var sub in _subModels)
				foreach (var param in sub.Params())
					param.PropertyChanged += handler;
		}
	}
}