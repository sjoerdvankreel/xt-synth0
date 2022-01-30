using System;

namespace Xt.Synth0.UI
{
	internal class MoveFxFocusEventArgs : EventArgs
	{
		internal bool Up { get; }
		internal bool Parsed { get; }
		internal MoveFxFocusEventArgs(bool parsed, bool up) 
		=> (Parsed, Up) = (parsed, up);
	}
}