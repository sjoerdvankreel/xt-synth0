using System;
using System.ComponentModel;

namespace Xt.Synth0.Model
{
	public class SynthModel : Model<SynthModel>
	{
		public event EventHandler ParamChanged;

		public UnitModel Unit1 { get; } = new();
		public UnitModel Unit2 { get; } = new();
		public UnitModel Unit3 { get; } = new();
		public TrackModel Track { get; } = new();
		public GlobalModel Global { get; } = new();

		IGroupModel[] Groups() => new IGroupModel[] {
			Global, Track, Unit1, Unit2, Unit3 };

		public SynthModel()
		{
			PropertyChangedEventHandler handler;
			handler = (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			foreach (var group in Groups())
				foreach (var param in group.Params())
					param.PropertyChanged += handler;
		}

		public override void CopyTo(SynthModel model)
		{
			for (int i = 0; i < Groups().Length; i++)
				Groups()[i].CopyTo(model.Groups()[i]);
		}
	}
}