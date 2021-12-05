namespace Xt.Synth0.Model
{
	public sealed class RateModel
	{
		public string Text { get; }
		public SampleRate Value { get; }

		public override string ToString() => Text;
		internal RateModel(SampleRate value, string text) 
		=> (Value, Text) = (value, text);
	}
}