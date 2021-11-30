using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class MenuUI
	{
		public static UIElement Make(UIModel model)
		{
			var result = new Menu();
			result.Items.Add(MakeFile(model));
			return result;
		}

		static MenuItem MakeItem(string header)
		{
			var result = new MenuItem();
			result.Header = header;
			return result;
		}

		static MenuItem MakeTheme()
		{
			var result = MakeItem("Theme");
			foreach (var theme in Enum.GetValues<ThemeType>())
			{
				var item = MakeItem(theme.ToString());
				item.IsCheckable = true;
				result.Items.Add(item);
			}
			return result;
		}

		static MenuItem MakeItem(ICommand command, string header, Action execute)
		{
			var result = MakeItem(header);
			result.Command = command;
			var binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += (s, e) => execute();
			result.CommandBindings.Add(binding);
			return result;
		}

		static UIElement MakeFile(UIModel model)
		{
			var result = MakeItem("_File");
			result.Items.Add(MakeItem(ApplicationCommands.New, "_New", model.RequestNew));
			result.Items.Add(MakeItem(ApplicationCommands.Open, "_Open", model.RequestOpen));
			result.Items.Add(new Separator());
			result.Items.Add(MakeItem(ApplicationCommands.Save, "_Save", model.RequestSave));
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, "Save _As", model.RequestSaveAs));
			result.Items.Add(new Separator());
			result.Items.Add(MakeTheme());
			return result;
		}
	}
}