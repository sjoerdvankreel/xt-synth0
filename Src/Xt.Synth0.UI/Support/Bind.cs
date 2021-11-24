using System;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Bind
	{
		internal static Binding Param(Param param)
		=> To(param, nameof(param.Value));

		// TODO
		public static Binding To(object source, string path)
		{
			var result = new Binding(path);
			result.Source = source;
			return result;
		}

		internal static Binding Format(Param param)
		{
			var result = Param(param);
			result.Converter = new Formatter(param.Info);
			return result;
		}

		// TODO
		public static MultiBinding Of(
			Func<object[], string> format, params Binding[] bindings)
		{
			var result = new MultiBinding();
			foreach (var binding in bindings)
				result.Bindings.Add(binding);
			result.Converter = new MultiFormatter(format);
			return result;
		}
	}
}