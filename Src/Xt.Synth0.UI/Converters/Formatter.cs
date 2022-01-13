using System;

namespace Xt.Synth0.UI
{
	class Formatter<T> : Converter<T, string>
	{
		readonly Func<T, string> _format;
		internal Formatter(Func<T, string> format) => _format = format;
		protected override string Convert(T value) => _format(value);
	}
}