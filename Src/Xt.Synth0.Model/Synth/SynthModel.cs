namespace Xt.Synth0.Model
{
	public class SynthModel
	{
		public UnitModel Unit1 { get; } = new();
		public UnitModel Unit2 { get; } = new();
		public UnitModel Unit3 { get; } = new();
		public GlobalModel Global { get; } = new();

		public IGroupModel[] Groups()
		=> new IGroupModel[] { Global, Unit1, Unit2, Unit3 };

		public void CopyTo(SynthModel model)
		{
			Unit1.CopyTo(model.Unit1);
			Unit2.CopyTo(model.Unit2);
			Unit3.CopyTo(model.Unit3);
			Global.CopyTo(model.Global);
		}
	}
}