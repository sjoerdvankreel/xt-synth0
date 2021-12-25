namespace Xt.Synth0.Model
{
	public sealed class SizeModel
	{
		public int Value { get; }
		public SampleSize Size { get; }

		public override string ToString() => Value.ToString();
		internal SizeModel(SampleSize size, int value)
		=> (Size, Value) = (size, value);
	}
}