namespace Xt.Synth0.Model
{
	public sealed class BufferModel
	{
		public int Value { get; }
		public BufferSize Size { get; }

		public override string ToString() => Value.ToString();
		internal BufferModel(BufferSize size, int value)
		=> (Size, Value) = (size, value);
	}
}