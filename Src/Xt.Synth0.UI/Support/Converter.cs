using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	internal class Converter<T, U> : IValueConverter
	{
		readonly Func<T, U> _convert;
		internal Converter(Func<T, U> convert)
		=> _convert = convert;

		public object Convert(object v, Type t, object p, CultureInfo c)
		=> _convert((T)v);
		public object ConvertBack(object v, Type t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}
}