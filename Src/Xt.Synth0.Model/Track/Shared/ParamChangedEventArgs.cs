using System;

namespace Xt.Synth0.Model
{
	public sealed class ParamChangedEventArgs : EventArgs
	{
		public int AutomationId { get; }
        public int ParamIndex { get; }

        internal ParamChangedEventArgs(int groupIndex, int paramIndex)
        => (AutomationId, ParamIndex) = (groupIndex, paramIndex);
    }
}