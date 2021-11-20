using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	internal class Converter<T> : IValueConverter
	{
		readonly Func<T, string> _convert;
		internal Converter(Func<T, string> convert)
		=> _convert = convert;

		public object Convert(object v, Type t, object p, CultureInfo c)
		=> _convert((T)v);
		public object ConvertBack(object v, Type t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}
}