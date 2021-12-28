using System;

namespace Xt.Synth0.Model
{
	public sealed class ParamChangedEventArgs: EventArgs
	{
		public int Index { get; }
		public int Value { get; }
		public bool IsAutomatable { get; }
		internal ParamChangedEventArgs(int index, bool isAutomatable, int value)
		=> (Index, IsAutomatable, Value) = (index, isAutomatable, value);
	}
}