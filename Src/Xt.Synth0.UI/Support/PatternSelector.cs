using System.Windows;

namespace Xt.Synth0.UI
{
	class PatternSelector : ValueConverter<int, UIElement>
	{
		readonly UIElement[] _elements;
		internal PatternSelector(UIElement[] elements) => _elements = elements;
		internal override UIElement Convert(int value) => _elements[value - 1];
	}
}