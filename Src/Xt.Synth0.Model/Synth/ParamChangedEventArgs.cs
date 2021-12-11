using System;

namespace Xt.Synth0.Model
{
	public sealed class ParamChangedEventArgs: EventArgs
	{
		public int Index { get; }
		public int Value { get; }
		internal ParamChangedEventArgs(int index, int value)
		=> (Index, Value) = (index, value);
	}
}