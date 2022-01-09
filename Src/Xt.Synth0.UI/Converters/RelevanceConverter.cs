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
		readonly ISubModel _model;
		internal RelevanceConverter(ISubModel model, Param param) => (_model, _param) = (model, param);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c) => throw new NotSupportedException();
		public object Convert(object[] vs, Type t, object p, CultureInfo c)
		=> _param.Info.Relevance.Relevant(_model, vs.Cast<int>().ToArray()) ? Visibility.Visible : Visibility.Hidden;
	}
}