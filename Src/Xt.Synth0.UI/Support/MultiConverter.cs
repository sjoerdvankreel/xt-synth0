using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	internal class MultiConverter<T> : IMultiValueConverter
	{
		readonly Func<object[], T> _convert;
		internal MultiConverter(Func<object[], T> convert)
		=> _convert = convert;

		public object Convert(object[] v, Type t, object p, CultureInfo c)
		=> _convert(v);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}
}