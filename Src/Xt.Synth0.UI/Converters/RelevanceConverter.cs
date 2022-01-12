using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class RelevanceConverter : IMultiValueConverter
	{
		readonly Param _param;
		readonly ISubModel _sub;
		internal RelevanceConverter(ISubModel sub, Param param) => (_sub, _param) = (sub, param);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c) => throw new NotSupportedException();
		public object Convert(object[] vs, Type t, object p, CultureInfo c)
		=> _param.Info.Relevance.Relevant(_sub, vs.Cast<int>().ToArray()) ? Visibility.Visible : Visibility.Hidden;
	}
}