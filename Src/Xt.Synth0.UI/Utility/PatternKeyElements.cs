using System;
using System.Windows;

namespace Xt.Synth0.UI
{
	class PatternKeyElements
	{
		internal UIElement Oct { get; set; }
		internal event EventHandler MoveOctFocus;
		internal void RequestMoveOctFocus() => MoveOctFocus?.Invoke(this, EventArgs.Empty);

		internal UIElement Amp { get; set; }
		internal event EventHandler MoveAmpFocus;
		internal void RequestMoveAmpFocus() => MoveAmpFocus?.Invoke(this, EventArgs.Empty);

		internal UIElement Note { get; set; }
		internal event EventHandler MoveNoteFocus;
		internal void RequestMoveNoteFocus() => MoveNoteFocus?.Invoke(this, EventArgs.Empty);
	}
}