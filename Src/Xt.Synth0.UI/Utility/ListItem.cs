namespace Xt.Synth0.UI
{
	public class ListItem
	{
		public int Value { get; }
		public string Display { get; }
		public override string ToString() => Display;
		public ListItem(int value, string display) => (Value, Display) = (value, display);
	}
}