using System;
using System.Globalization;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class OctFormatter : IMultiValueConverter
	{
		readonly PatternKey _model;
		internal OctFormatter(PatternKey model) => _model = model;

		public object[] ConvertBack(object value, Type[] targetTypes,
			object parameter, CultureInfo culture)
		=> throw new NotSupportedException();

		public object Convert(object[] values, Type targetType,
			object parameter, CultureInfo culture)
		{
			var note = (int)values[0];
			var oct = (int)values[1];
			if (note >= (int)PatternNote.C) return _model.Oct.Info.Format(oct);
			return new string(_model.Note.Info.Format(note)[0], 1);
		}
	}
}