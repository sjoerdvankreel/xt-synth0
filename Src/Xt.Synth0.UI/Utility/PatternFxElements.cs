using System;
using System.Windows;

namespace Xt.Synth0.UI
{
	class PatternFxElements
	{
		internal UIElement Value { get; set; }
		internal event EventHandler<MoveFxFocusEventArgs> MoveValueFocus;
		internal void RequestMoveValueFocus(bool parsed, bool up) 
		=> MoveValueFocus?.Invoke(this, new MoveFxFocusEventArgs(parsed, up));

		internal UIElement Target { get; set; }
		internal event EventHandler<MoveFxFocusEventArgs> MoveTargetFocus;
		internal void RequestMoveTargetFocus(bool parsed, bool up) 
		=> MoveTargetFocus?.Invoke(this, new MoveFxFocusEventArgs(parsed, up));
	}
}