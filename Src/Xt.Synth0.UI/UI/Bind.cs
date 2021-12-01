using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Bind
	{
		internal static Binding To(
			object source, string path)
		{
			var result = new Binding(path);
			result.Source = source;
			return result;
		}

		internal static Binding To(object source,
			string path, IValueConverter converter)
		{
			var result = To(source, path);
			result.Converter = converter;
			return result;
		}

		internal static Binding To(Param param)
		{
			var result = new Binding(nameof(Param.Value));
			result.Source = param;
			return result;
		}

		internal static Binding Show(Param param, int min)
		{
			var result = To(param);
			result.Converter = new ShowConverter(min);
			return result;
		}

		internal static Binding Format(Param param)
		{
			var result = To(param);
			result.Converter = new ParamFormatter(param.Info);
			return result;
		}

		internal static MultiBinding To(Param first,
			Param second, MultiConverter<int, int, string> formatter)
		{
			var result = new MultiBinding();
			result.Converter = formatter;
			result.Bindings.Add(To(first));
			result.Bindings.Add(To(second));
			return result;
		}

		public static MultiBinding To(object source1, string path1,
			object source2, string path2, IMultiValueConverter formatter)
		{
			var result = new MultiBinding();
			result.Converter = formatter;
			result.Bindings.Add(To(source1, path1));
			result.Bindings.Add(To(source2, path2));
			return result;
		}
	}
}