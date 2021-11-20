using System;
using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class Bind
	{
		internal static Binding To<T>(Param<T> param)
		{
			var result = new Binding(nameof(param.Value));
			result.Source = param;
			return result;
		}

		internal static Binding To<T>(Param<T> param, Func<T, string> display)
		{
			var result = To(param);
			result.Converter = new Converter<T>(display);
			return result;
		}
	}
}