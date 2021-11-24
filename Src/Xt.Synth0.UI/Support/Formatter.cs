using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	internal class Formatter : IValueConverter
	{
		readonly Func<int, string> _format;
		internal Formatter(Func<int, string> format) 
		=> _format = format;
		public object Convert(object v, Type t, object p, CultureInfo c)
		=> _format((int)v);
		public object ConvertBack(object v, Type t, object p, CultureInfo c) 
		=> throw new NotSupportedException();
	}
}