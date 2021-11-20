using System;
using System.ComponentModel;

namespace Xt.Synth0.Model
{
	public class SynthModel
	{
		public event EventHandler ParamChanged;

		public UnitModel Unit1 { get; } = new();
		public UnitModel Unit2 { get; } = new();
		public UnitModel Unit3 { get; } = new();
		public GlobalModel Global { get; } = new();

		IGroupModel[] Groups() => new IGroupModel[] { Global, Unit1, Unit2, Unit3 };

		public SynthModel()
		{
			PropertyChangedEventHandler handler;
			handler = (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			foreach (var group in Groups())
				foreach (var param in group.Params())
					param.PropertyChanged += handler;
		}

		public void CopyTo(SynthModel model)
		{
			Unit1.CopyTo(model.Unit1);
			Unit2.CopyTo(model.Unit2);
			Unit3.CopyTo(model.Unit3);
			Global.CopyTo(model.Global);
		}
	}
}