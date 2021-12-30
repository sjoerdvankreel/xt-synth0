namespace Xt.Synth0.Model
{
	public sealed class DeviceModel
	{
		public string Id { get; }
		public string Name { get; }

		public override string ToString() => Name;
		public DeviceModel(string id, string name) => (Id, Name) = (id, name);
	}
}