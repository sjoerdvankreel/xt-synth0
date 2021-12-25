namespace Xt.Synth0.Model
{
	public sealed class DepthModel
	{
		public int Value { get; }
		public BitDepth Depth { get; }

		public override string ToString() => Value.ToString();
		internal DepthModel(BitDepth depth, int value)
		=> (Depth, Value) = (depth, value);
	}
}