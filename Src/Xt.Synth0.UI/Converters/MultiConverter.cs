using System;
using System.Globalization;
using System.Windows.Data;

namespace Xt.Synth0.UI
{
	public abstract class MultiConverter : IMultiValueConverter
	{
		public abstract object Convert(object[] v, Type t, object p, CultureInfo c);
		public object[] ConvertBack(object v, Type[] t, object p, CultureInfo c)
		=> throw new NotSupportedException();
	}

	public abstract class MultiConverter<T, U, V> : MultiConverter
	{
		protected abstract V Convert(T arg1, U arg2);
		public override object Convert(object[] v, Type t, object p, CultureInfo c)
		=> Convert((T)v[0], (U)v[1]);
	}

	public abstract class MultiConverter<T, U, V, W> : MultiConverter
	{
		internal abstract W Convert(T arg1, U arg2, V arg3);
		public override object Convert(object[] v, Type t, object p, CultureInfo c)
		=> Convert((T)v[0], (U)v[1], (V)v[2]);
	}

	public abstract class MultiConverter<T, U, V, W, X> : MultiConverter
	{
		internal abstract X Convert(T arg1, U arg2, V arg3, W arg4);
		public override object Convert(object[] v, Type t, object p, CultureInfo c)
		=> Convert((T)v[0], (U)v[1], (V)v[2], (W)v[3]);
	}
}