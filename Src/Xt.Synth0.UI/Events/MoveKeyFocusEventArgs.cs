using System;

namespace Xt.Synth0.UI
{
	internal class MoveKeyFocusEventArgs : EventArgs
	{
		internal bool Up { get; }
		internal MoveKeyFocusEventArgs(bool up) => Up = up;
	}
}