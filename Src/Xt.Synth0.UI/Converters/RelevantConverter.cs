using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class RelevantConverter : IMultiValueConverter
	{
		readonly Param _param;
		internal RelevantConverter(Param param) => _param = param;
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
		=> throw new NotSupportedException();

		public object Convert(object[] vs, Type t, object p, CultureInfo c)
		=> _param.Info.IsRelevant(vs.Cast<int>().ToArray()) ? Visibility.Visible : Visibility.Hidden;
	}
}