using System;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Bind
	{
		internal static Binding To(Param param)
		=> To(param, nameof(param.Value));

		public static Binding To(object source, string path)
		{
			var result = new Binding(path);
			result.Source = source;
			return result;
		}

		internal static Binding To(Param param, Func<int, string> format)
		{
			var result = To(param);
			result.Converter = new Formatter(format);
			return result;
		}

		public static MultiBinding Of(Func<object[], string> format, params Binding[] bindings)
		{
			var result = new MultiBinding();
			foreach (var binding in bindings)
				result.Bindings.Add(binding);
			result.Converter = new MultiFormatter(format);
			return result;
		}
	}
}