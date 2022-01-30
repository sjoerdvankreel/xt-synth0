using System;
using System.Windows;

namespace Xt.Synth0.UI
{
	class PatternKeyElements
	{
		internal UIElement Oct { get; set; }
		internal event EventHandler<MoveKeyFocusEventArgs> MoveOctFocus;
		internal void RequestMoveOctFocus(bool up) 
		=> MoveOctFocus?.Invoke(this, new MoveKeyFocusEventArgs(up));

		internal UIElement Amp { get; set; }
		internal event EventHandler<MoveKeyFocusEventArgs> MoveAmpFocus;
		internal void RequestMoveAmpFocus(bool up) 
		=> MoveAmpFocus?.Invoke(this, new MoveKeyFocusEventArgs(up));

		internal UIElement Note { get; set; }
		internal event EventHandler<MoveKeyFocusEventArgs> MoveNoteFocus;
		internal void RequestMoveNoteFocus(bool up) 
		=> MoveNoteFocus?.Invoke(this, new MoveKeyFocusEventArgs(up));
	}
}