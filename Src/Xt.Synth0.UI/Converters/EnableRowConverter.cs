using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class EnableRowConverter : MultiConverter<ThemeType, string, int, Brush>
	{
		readonly int _row;
		readonly SettingsModel _settings;
		internal EnableRowConverter(SettingsModel settings, int row)
		=> (_settings, _row) = (settings, row);

		protected override Brush Convert(ThemeType type, string color, int rows)
		{
			var resources = Utility.GetThemeResources(_settings, ThemeGroup.EditPattern);
			var enabled = (Brush)resources[Utility.RowEnabledKey];
			var disabled = (Brush)resources[Utility.RowDisabledKey];
			return _row < rows ? enabled : disabled;
		}
	}
}