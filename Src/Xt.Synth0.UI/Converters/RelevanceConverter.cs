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
		readonly IParamGroupModel _part;
		internal RelevanceConverter(IParamGroupModel part, Param param) => (_part, _param) = (part, param);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c) => throw new NotSupportedException();
		public object Convert(object[] vs, Type t, object p, CultureInfo c)
		=> _param.Info.Relevance.Relevant(_part, vs.Cast<int>().ToArray()) ? Visibility.Visible : Visibility.Hidden;
	}
}