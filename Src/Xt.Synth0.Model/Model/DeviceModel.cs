namespace Xt.Synth0.Model
{
	public sealed class DeviceModel
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public override string ToString() => Name;
	}
}