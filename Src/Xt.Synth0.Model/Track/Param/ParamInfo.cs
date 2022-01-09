using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	unsafe delegate int* Address(void* parent);
	public enum ParamControl { Toggle, List, Knob };
	public enum ParamType { Toggle, List, Lin, Quad, Exp };

	public sealed class ParamInfo
	{
		public int Min { get; }
		public int Max { get; }
		public int Default { get; }
		public string Name { get; }
		public ParamType Type { get; }

		int? _maxDisplayLength;
		readonly Address _address;
		readonly int[][] _relevantWhen;
		readonly Func<int, string> _display;
		readonly Func<ISubModel, Param[]> _relevant;

		public ParamControl Control => Type switch
		{
			ParamType.Exp => ParamControl.Knob,
			ParamType.Lin => ParamControl.Knob,
			ParamType.Quad => ParamControl.Knob,
			ParamType.List => ParamControl.List,
			ParamType.Toggle => ParamControl.Toggle,
			_ => throw new InvalidOperationException()
		};

		public string Format(int value) => Type switch
		{
			ParamType.Lin => _display(value),
			ParamType.List => _display(value),
			ParamType.Exp => (1 << value).ToString(),
			ParamType.Quad => (value * value).ToString(),
			ParamType.Toggle => value == 0 ? "Off" : "On",
			_ => throw new InvalidOperationException()
		};

		public bool IsRelevant(int[] relevantValues)
		{
			for (int i = 0; i < _relevantWhen.Length; i++)
				if (!_relevantWhen[i].Contains(relevantValues[i]))
					return false;
			return true;
		}

		public unsafe int* Address(void* native) => _address(native);
		public Param[] Relevant(ISubModel sub) => _relevant?.Invoke(sub);
		public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
		int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);

		ParamInfo(ParamType type, Address address, string name, int min, int max, int @default,
			Func<int, string> display, Func<ISubModel, Param[]> relevant, int[][] relevantWhen)
		=> (Type, _address, Name, Min, Max, Default, _display, _relevant, _relevantWhen)
		= (type, address, name, min, max, @default, display, relevant, relevantWhen);

		internal static ParamInfo Lin(Address address, string name, string[] display,
			Func<ISubModel, Param[]> relevant = null, params int[][] relevantWhen)
		=> new ParamInfo(ParamType.Lin, address, name, 0, display.Length - 1, 0, x => display[x], relevant, relevantWhen);

		internal static ParamInfo Toggle(Address address, string name, bool @default,
			Func<ISubModel, Param[]> relevant = null, params int[][] relevantWhen)
		=> new ParamInfo(ParamType.Toggle, address, name, 0, 1, @default ? 1 : 0, null, relevant, relevantWhen);

		internal static ParamInfo Exp(Address address, string name, int min, int max,
			int @default, Func<ISubModel, Param[]> relevant = null, params int[][] relevantWhen)
		=> new ParamInfo(ParamType.Exp, address, name, min, max, @default, null, relevant, relevantWhen);

		internal static ParamInfo Quad(Address address, string name, int min, int max,
			int @default, Func<ISubModel, Param[]> relevant = null, params int[][] relevantWhen)
		=> new ParamInfo(ParamType.Quad, address, name, min, max, @default, null, relevant, relevantWhen);

		internal static ParamInfo Lin(Address address, string name, int min, int max,
			int @default, Func<int, string> display = null, Func<ISubModel, Param[]> relevant = null, params int[][] relevantWhen)
		=> new ParamInfo(ParamType.Lin, address, name, min, max, @default, display ?? (x => x.ToString()), relevant, relevantWhen);

		internal static ParamInfo List<TEnum>(Address address, string name, string[] display = null,
			Func<ISubModel, Param[]> relevant = null, params int[][] relevantWhen) where TEnum : struct, Enum
		=> new ParamInfo(ParamType.List, address, name, 0, Enum.GetValues<TEnum>().Length - 1, 0,
			display != null ? x => display[x] : x => Enum.GetNames<TEnum>()[x], relevant, relevantWhen);
	}
}