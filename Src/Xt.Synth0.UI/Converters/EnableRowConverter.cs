using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class EnableRowConverter : MultiConverter<ThemeType, int, Brush>
	{
		readonly int _row;
		internal EnableRowConverter(int row) => _row = row;

		protected override Brush Convert(ThemeType theme, int rows)
		{
			var resources = Utility.GetThemeResources(theme);
			var enabled = (Brush)resources[Utility.RowEnabledKey];
			var disabled = (Brush)resources[Utility.RowDisabledKey];
			return _row < rows ? enabled : disabled;
		}
	}
}