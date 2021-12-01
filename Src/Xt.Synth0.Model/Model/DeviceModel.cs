namespace Xt.Synth0.Model
{
	public sealed class DeviceModel : ViewModel
	{
		string _id;
		public string Id
		{
			get => _id;
			set => Set(ref _id, value);
		}

		string _name;
		public string Name
		{
			get => _name;
			set => Set(ref _name, value);
		}
	}
}