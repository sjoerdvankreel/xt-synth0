using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	unsafe delegate int* Address(void* parent);
	public enum ParamType { Toggle, List, Lin, Quad, Exp };

	public sealed class ParamInfo
	{
		int? _maxDisplayLength;
		readonly Address _address;
		readonly Func<int, string> _display;

		public int Min { get; }
		public int Max { get; }
		public int Default { get; }
		public string Name { get; }
		public ParamType Type { get; }

		public string Format(int value) => Type switch
		{
			ParamType.Lin => _display(value),
			ParamType.List => _display(value),
			ParamType.Exp => (1 << value).ToString(),
			ParamType.Quad => (value * value).ToString(),
			ParamType.Toggle => value == 0 ? "Off" : "On",
			_ => throw new InvalidOperationException()
		};

		public unsafe int* Address(void* native) => _address(native);
		public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
		int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);

		internal static ParamInfo Lin(Address address, string name, string[] display)
		=> new ParamInfo(ParamType.Lin, address, name, 0, display.Length - 1, 0, x => display[x]);
		internal static ParamInfo Toggle(Address address, string name, bool @default)
		=> new ParamInfo(ParamType.Toggle, address, name, 0, 1, @default ? 1 : 0, null);
		internal static ParamInfo Exp(Address address, string name, int min, int max, int @default)
		=> new ParamInfo(ParamType.Exp, address, name, min, max, @default, null);
		internal static ParamInfo Quad(Address address, string name, int min, int max, int @default)
		=> new ParamInfo(ParamType.Quad, address, name, min, max, @default, null);
		internal static ParamInfo Lin(Address address, string name, int min, int max, int @default, Func<int, string> display = null)
		=> new ParamInfo(ParamType.Lin, address, name, min, max, @default, display ?? (x => x.ToString()));
		internal static ParamInfo List<TEnum>(Address address, string name, string[] display = null) where TEnum : struct, Enum
		=> new ParamInfo(ParamType.List, address, name, 0, Enum.GetValues<TEnum>().Length - 1, 0, display != null ? x => display[x] : x => Enum.GetNames<TEnum>()[x]);
		ParamInfo(ParamType type, Address address, string name, int min, int max, int @default, Func<int, string> display)
		=> (Type, _address, Name, Min, Max, Default, _display) = (type, address, name, min, max, @default, display);
	}
}