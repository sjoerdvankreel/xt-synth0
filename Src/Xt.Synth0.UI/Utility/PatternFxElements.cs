using System;
using System.Windows;

namespace Xt.Synth0.UI
{
	class PatternFxElements
	{
		internal UIElement Value { get; set; }
		internal event EventHandler MoveValueFocus;
		internal void RequestMoveValueFocus() => MoveValueFocus?.Invoke(this, EventArgs.Empty);

		internal UIElement Target { get; set; }
		internal event EventHandler MoveTargetFocus;
		internal void RequestMoveTargetFocus() => MoveTargetFocus?.Invoke(this, EventArgs.Empty);
	}
}