using System.Windows;

namespace Xt.Synth0.UI
{
	class VisibilityConverter : Converter<bool, Visibility>
	{
		readonly bool _value;
		internal VisibilityConverter(bool value)
		=> _value = value;
		internal override Visibility Convert(bool value)
		=> value == _value ? Visibility.Visible : Visibility.Collapsed;
	}
}