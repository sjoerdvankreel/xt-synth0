using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class EnableRowConverter : MultiConverter<string, int, Brush>
	{
		readonly int _row;
		readonly SettingsModel _settings;
		internal EnableRowConverter(SettingsModel settings, int row)
		=> (_settings, _row) = (settings, row);

		protected override Brush Convert(string patternColor, int rows)
		{
			var resources = Utility.GetThemeResources(_settings, ThemeGroup.Pattern);
			var enabled = (Brush)resources[Utility.RowEnabledKey];
			var disabled = (Brush)resources[Utility.RowDisabledKey];
			return _row < rows ? enabled : disabled;
		}
	}
}