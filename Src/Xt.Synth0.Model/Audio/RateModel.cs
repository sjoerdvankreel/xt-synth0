namespace Xt.Synth0.Model
{
	public sealed class RateModel
	{
		public int Value { get; }
		public SampleRate Rate { get; }

		public override string ToString() => Value.ToString();
		internal RateModel(SampleRate rate, int value)
		=> (Rate, Value) = (rate, value);
	}
}