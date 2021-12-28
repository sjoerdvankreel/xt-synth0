namespace Xt.Synth0
{
	internal struct AutomationAction
	{
		internal int Param { get; }
		internal int Value { get; }
		internal AutomationAction(int param, int value)
		=> (Param, Value) = (param, value);
	}
}