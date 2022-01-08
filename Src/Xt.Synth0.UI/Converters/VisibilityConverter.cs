using System.Windows;

namespace Xt.Synth0.UI
{
	class VisibilityConverter : Converter<bool, Visibility>
	{
		readonly bool _value;
		readonly bool _hidden;
		internal VisibilityConverter(bool hidden, bool value)
		=> (_hidden, _value) = (hidden, value);
		protected override Visibility Convert(bool value)
		=> value == _value ? Visibility.Visible : _hidden ? Visibility.Hidden : Visibility.Collapsed;
	}
}