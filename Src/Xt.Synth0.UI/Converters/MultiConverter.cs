using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	public abstract class MultiConverter<T, U, V> : IMultiValueConverter
	{
		protected abstract V Convert(T arg1, U arg2);

		public object Convert(object[] v, Type t, object p, CultureInfo c)
		=> Convert((T)v[0], (U)v[1]);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}
}