using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	abstract class Converter<T, U> : IValueConverter
	{
		protected abstract U Convert(T value);
		protected virtual T ConvertBack(U value) => throw new NotSupportedException();
		public object Convert(object v, Type t, object p, CultureInfo c) => Convert((T)v);
		public object ConvertBack(object v, Type t, object p, CultureInfo c) => ConvertBack((U)v);
	}
}