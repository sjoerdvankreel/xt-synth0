using System;

namespace Xt.Synth0.Model
{
	public sealed class ParamChangedEventArgs : EventArgs
	{
		public int Index { get; }
		internal ParamChangedEventArgs(int index) => Index = index;
	}
}