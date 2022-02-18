using System;

namespace Xt.Synth0.Model
{
	public sealed class EnumModel<T>
		where T : struct, Enum
	{
		public T Enum { get; }
		public string Display { get; }

		public override string ToString() => Display;
		internal EnumModel(T @enum, string display) => (Enum, Display) = (@enum, display);
	}
}