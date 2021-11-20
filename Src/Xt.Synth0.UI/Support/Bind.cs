using System;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Bind
	{
		internal static Binding To<T>(Param<T> param)
		=> To(param, nameof(param.Value));

		public static Binding To(object source, string path)
		{
			var result = new Binding(path);
			result.Source = source;
			return result;
		}

		internal static Binding To<T>(Param<T> param, Func<T, string> display)
		{
			var result = To(param);
			result.Converter = new Converter<T>(display);
			return result;
		}

		public static Binding To<T>(object source, string path, Func<T, string> display)
		{
			var result = To(source, path);
			result.Converter = new Converter<T>(display);
			return result;
		}
	}
}