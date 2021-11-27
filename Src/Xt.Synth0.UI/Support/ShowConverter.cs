using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	class ShowConverter : IValueConverter
	{
		readonly int _min;
		internal ShowConverter(int min)
		=> _min = min;

		public object ConvertBack(object value, Type targetType,
			object parameter, CultureInfo culture)
			=> throw new NotSupportedException();

		public object Convert(object value, Type targetType,
			object parameter, CultureInfo culture)
		=> (int)value >= _min ? Visibility.Visible : Visibility.Collapsed;
	}
}