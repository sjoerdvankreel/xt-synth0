using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	unsafe delegate int* Address(void* parent);
	public enum ParamControl { Toggle, List, Knob };
	public enum ParamType { Toggle, List, Lin, Time, Exp };

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
		public bool Automatable { get; }
		public string Description { get; }
		public IRelevance Relevance { get; }

		public ParamControl Control => Type switch
		{
			ParamType.Exp => ParamControl.Knob,
			ParamType.Lin => ParamControl.Knob,
			ParamType.Time => ParamControl.Knob,
			ParamType.List => ParamControl.List,
			ParamType.Toggle => ParamControl.Toggle,
			_ => throw new InvalidOperationException()
		};

		public string Format(int value) => Type switch
		{
			ParamType.Lin => _display(value),
			ParamType.List => _display(value),
			ParamType.Time => FormatTime(value),
			ParamType.Exp => (1 << value).ToString(),
			ParamType.Toggle => value == 0 ? "Off" : "On",
			_ => throw new InvalidOperationException()
		};

		string FormatTime(int value)
		{
			int ms = value * value;
			if (ms < 1000) return $"{ms}m";
			if (ms < 10000) return $"{(ms / 1000.0).ToString("N1")}s";
			if (ms == 10000) return "10s";
			throw new InvalidOperationException();
		}

		public string Range(bool hex)
		{
			bool lin = Type != ParamType.Time && Type != ParamType.Exp;
			string min = hex ? Min.ToString("X2") : Min.ToString();
			string max = hex ? Max.ToString("X2") : Max.ToString();
			return lin ? $"{min} .. {max}" : $"{min} ({Format(Min)}) .. {max} ({Format(Max)})";
		}

		public unsafe int* Address(void* native) => _address(native);
		public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
		int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);

		ParamInfo(ParamType type, Address address, string name, string description, bool automatable,
			int min, int max, int @default, Func<int, string> display, IRelevance relevance)
		{
			(Type, _address, Name, Description, Automatable, Min, Max, Default, _display, Relevance)
			= (type, address, name, description, automatable, min, max, @default, display, relevance);
			if (min < 0 || max > 255 || min >= max || @default < min || @default > max)
				throw new InvalidOperationException();
		}

		internal static ParamInfo Exp(Address address, string name, string description,
			bool automatable, int max, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Exp, address, name, description, automatable, 0, max, @default, null, relevance);

		internal static ParamInfo Time(Address address, string name, string description,
			bool automatable, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Time, address, name, description, automatable, 0, 100, @default, null, relevance);

		internal static ParamInfo Toggle(Address address, string name, string description,
			bool automatable, bool @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Toggle, address, name, description, automatable, 0, 1, @default ? 1 : 0, null, relevance);

		internal static ParamInfo Select(Address address, string name, string description,
			bool automatable, string[] display, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, name, description, automatable, 0, display.Length - 1, 0, x => display[x], relevance);

		internal static ParamInfo Mix(Address address, string name, string description, bool automatable,
			Func<int, string> display = null, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, name, description, automatable, 1, 255, 128, display ?? (x => x.ToString()), relevance);

		internal static ParamInfo Level(Address address, string name, string description, bool automatable,
			int @default, Func<int, string> display = null, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, name, description, automatable, 0, 255, @default, display ?? (x => x.ToString()), relevance);

		internal static ParamInfo Select(Address address, string name, string description, bool automatable,
			int min, int max, int @default, Func<int, string> display = null, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, name, description, automatable, min, max, @default, display ?? (x => x.ToString()), relevance);

		internal static ParamInfo List<TEnum>(Address address, string name, string description,
			bool automatable, string[] display = null, IRelevance relevance = null) where TEnum : struct, Enum
		=> new ParamInfo(ParamType.List, address, name, description, automatable, 0, Enum.GetValues<TEnum>().Length - 1, 0,
			display != null ? x => display[x] : x => Enum.GetNames<TEnum>()[x], relevance);
	}
}