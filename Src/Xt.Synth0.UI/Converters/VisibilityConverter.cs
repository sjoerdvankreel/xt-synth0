using System.Windows;

namespace Xt.Synth0.UI
{
	class VisibilityConverter<T> : Converter<T, Visibility>
	{
		readonly T _value;
		readonly bool _hidden;
		internal VisibilityConverter(bool hidden, T value)
		=> (_hidden, _value) = (hidden, value);
		protected override Visibility Convert(T value)
		=> value.Equals(_value) ? Visibility.Visible : _hidden ? Visibility.Hidden : Visibility.Collapsed;
	}
}