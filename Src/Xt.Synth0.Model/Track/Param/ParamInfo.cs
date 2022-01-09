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
		readonly Relevance[] _relevance;
		readonly Func<int, string> _display;

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

		public bool IsRelevant(int[] values)
		{
			for (int i = 0; i < _relevance.Length; i++)
				if (!_relevance[i].Values.Contains(values[i]))
					return false;
			return true;
		}

		public unsafe int* Address(void* native) => _address(native);
		public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
		public Param[] RelevanceParams(ISubModel sub) => _relevance?.Select(r => r.Param(sub)).ToArray();
		int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);

		ParamInfo(ParamType type, Address address, string name, int min, int max, 
			int @default, Func<int, string> display, Relevance[] relevance)
		=> (Type, _address, Name, Min, Max, Default, _display, _relevance) 
		= (type, address, name, min, max, @default, display, relevance);

		internal static ParamInfo List<TEnum>(Address address, string name,
			string[] display = null, params Relevance[] relevance) where TEnum : struct, Enum
		=> new ParamInfo(ParamType.List, address, name, 0, Enum.GetValues<TEnum>().Length - 1, 0,
			display != null ? x => display[x] : x => Enum.GetNames<TEnum>()[x], relevance);

		internal static ParamInfo Lin(Address address, string name, int min, int max, 
			int @default, Func<int, string> display = null, params Relevance[] relevance)
		=> new ParamInfo(ParamType.Lin, address, name, min, max, @default, display ?? (x => x.ToString()), relevance);

		internal static ParamInfo Toggle(Address address, string name, bool @default, params Relevance[] relevance)
		=> new ParamInfo(ParamType.Toggle, address, name, 0, 1, @default ? 1 : 0, null, relevance);
		internal static ParamInfo Lin(Address address, string name, string[] display, params Relevance[] relevance)
		=> new ParamInfo(ParamType.Lin, address, name, 0, display.Length - 1, 0, x => display[x], relevance);
		internal static ParamInfo Exp(Address address, string name, int min, int max, int @default, params Relevance[] relevance)
		=> new ParamInfo(ParamType.Exp, address, name, min, max, @default, null, relevance);
		internal static ParamInfo Quad(Address address, string name, int min, int max, int @default, params Relevance[] relevance)
		=> new ParamInfo(ParamType.Quad, address, name, min, max, @default, null, relevance);
	}
}