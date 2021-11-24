using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	internal class MultiFormatter : IMultiValueConverter
	{
		readonly Func<object[], string> _format;
		internal MultiFormatter(Func<object[], string> format)
		=> _format = format;
		public object Convert(object[] v, Type t, object p, CultureInfo c)
		=> _format(v);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}
}