using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class MenuUI
	{
		public static event EventHandler New;
		public static event EventHandler Open;
		public static event EventHandler Save;
		public static event EventHandler SaveAs;
		public static event EventHandler ShowSettings;

		public static UIElement Make(AudioModel model)
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

		static MenuItem MakeItem(
			ICommand command, string header, Action execute)
		{
			var result = MakeItem(header);
			result.Command = command;
			var binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += (s, e) => execute();
			result.CommandBindings.Add(binding);
			return result;
		}

		static MenuItem MakeItem(ICommand command,
			AudioModel model, string header, Action execute)
		{
			var result = MakeItem(command, header, execute);
			var binding = Bind.To(model, nameof(model.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement MakeFile(AudioModel model)
		{
			var result = MakeItem("_File");
			var doNew = () => New(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.New, model, "_New", doNew));
			var doOpen = () => Open(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.Open, model, "_Open", doOpen));
			result.Items.Add(new Separator());
			var doSave = () => Save(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.Save, "_Save", doSave));
			var doSaveAs = () => SaveAs(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, "Save _As", doSaveAs));
			result.Items.Add(new Separator());
			var doShowSettings = () => ShowSettings(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, model, "Settings", doShowSettings));
			return result;
		}
	}
}