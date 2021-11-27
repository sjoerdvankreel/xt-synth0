using System;
using System.Globalization;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class Formatter : IValueConverter
	{
		readonly ParamInfo _info;
		internal Formatter(ParamInfo info)
		=> _info = info;
		public object Convert(object v, Type t, object p, CultureInfo c)
		=> _info.Format((int)v).PadRight(_info.MaxDisplayLength, ' ');
		public object ConvertBack(object v, Type t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}
}