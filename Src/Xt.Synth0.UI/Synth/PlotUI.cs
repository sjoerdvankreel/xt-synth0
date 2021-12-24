using System;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PlotUI
	{
		internal static UIElement Make(AppModel model)
		{
			var result = new GroupBox();
			result.Header = "Plot";
			model.Synth.ParamChanged += (s, e) => UpdatePlot(result, model);
			model.Settings.PropertyChanged += (s, e) => UpdatePlot(result, model);
			return result;
		}

		static void UpdatePlot(ContentControl control, AppModel model)
		{
			var text = new TextBlock();
			text.Text = new Random().NextDouble().ToString();
			control.Content = text;
		}
	}
}