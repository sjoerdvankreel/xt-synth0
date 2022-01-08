using System.Windows.Data;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Bind
	{
		public static Binding To(
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

		public static Binding Show(
			object source, string path, int min)
		{
			var result = To(source, path);
			result.Converter = new ShowConverter(min);
			return result;
		}

		internal static MultiBinding ShowRow(
			Param rows, int row, Param param, int min)
		{
			var rowBinding = To(rows);
			var paramBinding = To(param);
			return To(new ShowRowConverter(row, min), rowBinding, paramBinding);
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

		public static MultiBinding To(
			IMultiValueConverter formatter, params Binding[] bindings)
		{
			var result = new MultiBinding();
			foreach (var b in bindings)
				result.Bindings.Add(b);
			result.Converter = formatter;
			return result;
		}
	}
}