using System.ComponentModel;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class EnableRowConverter : MultiConverter<ThemeType, int, Brush>
	{
		Brush _enabled;
		Brush _disabled;
		readonly int _row;

		internal EnableRowConverter(SettingsModel settings, int row)
		{
			_row = row;
			UpdateBrushes(settings.Theme);
			settings.PropertyChanged += (s, e) => OnSettingsPropertyChanged(settings, e);
		}

		protected override Brush Convert(ThemeType theme, int rows)
		=> _row < rows ? _enabled : _disabled;

		void UpdateBrushes(ThemeType theme)
		{
			var resources = Utility.GetThemeResources(theme);
			_enabled = (Brush)resources[Utility.RowEnabledKey];
			_disabled = (Brush)resources[Utility.RowDisabledKey];
		}

		void OnSettingsPropertyChanged(SettingsModel settings, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(SettingsModel.Theme))
				UpdateBrushes(settings.Theme);
		}
	}
}