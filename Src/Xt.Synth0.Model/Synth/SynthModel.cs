using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class SynthModel : Model
	{
		public const int CurrentVersion = 1;
		public event EventHandler ParamChanged;

		public int Version { get; set; } = CurrentVersion;

		public PatternModel Pattern { get; } = new();
		public UnitModel Unit1 { get; } = new(nameof(Unit1));
		public UnitModel Unit2 { get; } = new(nameof(Unit2));
		public UnitModel Unit3 { get; } = new(nameof(Unit3));
		public GlobalModel Global { get; } = new(nameof(Global));
		public EditorModel Editor { get; } = new(nameof(Editor));

		readonly SubModel[] _subModels;
		readonly AutoParam[] _autoParams;
		public AutoParam[] AutoParams() => _autoParams;

		public override void CopyTo(Model model)
		{
			var synth = (SynthModel)model;
			for (int s = 0; s < _subModels.Length; s++)
				_subModels[s].CopyTo(synth._subModels[s]);
		}

		public SynthModel()
		{
			List<AutoParam> autoParams = new();
			PropertyChangedEventHandler handler;
			handler = (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			_subModels = new SubModel[] { Unit1, Unit2, Unit3, Global, Editor, Pattern };
			foreach (var sub in _subModels)
				foreach (var param in sub.Params())
					param.PropertyChanged += handler;
			foreach (var sub in _subModels.OfType<GroupModel>().Where(m => m.Automation))
				foreach (var p in sub.Params())
					autoParams.Add(new AutoParam(sub.Name(), p));
			_autoParams = autoParams.ToArray();
		}
	}
}