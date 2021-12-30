using System;

namespace Xt.Synth0.Model
{
	public sealed class EnumModel<T>
		where T : struct, Enum
	{
		public T Enum { get; }
		public int Value { get; }

		public override string ToString() => Value.ToString();
		internal EnumModel(T @enum, int value) => (Enum, Value) = (@enum, value);
	}
}