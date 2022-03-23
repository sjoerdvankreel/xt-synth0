using System;

namespace Xt.Synth0.Model
{
	public sealed class ParamChangedEventArgs : EventArgs
	{
		public int Target { get; }
        internal ParamChangedEventArgs(int target) => Target = target;
    }
}