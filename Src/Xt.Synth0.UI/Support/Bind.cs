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

		internal static Binding To<T>(Param param, Func<int, T> convert)
		{
			var result = To(param);
			result.Converter = new Converter<int, T>(convert);
			return result;
		}

		public static Binding To<T, U>(object source, string path, Func<T, U> convert)
		{
			var result = To(source, path);
			result.Converter = new Converter<T, U>(convert);
			return result;
		}

		public static MultiBinding Of<T>(Func<object[], T> convert, params Binding[] bindings)
		{
			var result = new MultiBinding();
			foreach (var binding in bindings)
				result.Bindings.Add(binding);
			result.Converter = new MultiConverter<T>(convert);
			return result;
		}
	}
}