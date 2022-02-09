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
		readonly Func<string, int> _load;
		readonly Func<int, string> _store;
		readonly Func<int, string> _display;

		public int Min { get; }
		public int Max { get; }
		public int Default { get; }
		public string Id { get; }
		public string Name { get; }
		public ParamType Type { get; }
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

		static string StoreEnum<TEnum>(int value) where TEnum : struct, Enum
		=> ((TEnum)(object)value).ToString();
		static int LoadEnum<TEnum>(string value) where TEnum : struct, Enum
		=> Enum.TryParse<TEnum>(value, out var result) ? (int)(object)result : 0;

		internal int Load(string value) => _load(value);
		internal string Store(int value) => _store(value);
		public unsafe int* Address(void* native) => _address(native);
		public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
		int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);

		ParamInfo(ParamType type, Address address, string id, string name, string description, int min, int max,
			int @default, Func<string, int> load, Func<int, string> store, Func<int, string> display, IRelevance relevance)
		{
			(Type, _address, Id, Name, Description, Min, Max, Default, _load, _store, _display, Relevance)
			= (type, address, id, name, description, min, max, @default, load, store, display, relevance);
			if (min < 0 || max > 255 || min >= max || @default < min || @default > max)
				throw new InvalidOperationException();
			_load ??= x => int.Parse(x);
			_store ??= x => x.ToString();
			_display ??= x => x.ToString();
		}

		internal static ParamInfo Mix(Address address, string id, string name,
			string description, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, id, name, description, 1, 255, 128, null, null, null, relevance);

		internal static ParamInfo Exp(Address address, string id, string name,
			string description, int max, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Exp, address, id, name, description, 0, max, @default, null, null, null, relevance);

		internal static ParamInfo Level(Address address, string id, string name,
			string description, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, id, name, description, 0, 255, @default, null, null, null, relevance);

		internal static ParamInfo Select(Address address, string id, string name,
			string description, int min, int max, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, id, name, description, min, max, @default, null, null, null, relevance);

		internal static ParamInfo Time(Address address, string id, string name,
			string description, int min, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Time, address, id, name, description, min, 100, @default, null, null, null, relevance);

		internal static ParamInfo Toggle(Address address, string id, string name,
			string description, bool @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Toggle, address, id, name, description, 0, 1, @default ? 1 : 0, null, null, null, relevance);

		internal static ParamInfo Select<TEnum>(Address address, string id, string name,
			string description, string[] display, IRelevance relevance = null) where TEnum : struct, Enum
		=> new ParamInfo(ParamType.Lin, address, id, name, description, 0,
			display.Length - 1, 0, LoadEnum<TEnum>, StoreEnum<TEnum>, x => display[x], relevance);

		internal static unsafe ParamInfo Step(Address address, string id, string name,
			string description, int min, int @default, IRelevance relevance = null)
		=> new ParamInfo(ParamType.Lin, address, id, name, description, min, SynthModel.SyncSteps.Length - 1,
			@default, null, null, val => SynthModel.SyncSteps[val].ToString(), relevance);

		internal static ParamInfo List<TEnum>(Address address, string id, string name,
			string description, string[] display = null, IRelevance relevance = null) where TEnum : struct, Enum
		=> new ParamInfo(ParamType.List, address, id, name, description, 0, Enum.GetValues<TEnum>().Length - 1, 0,
			LoadEnum<TEnum>, StoreEnum<TEnum>, display != null ? x => display[x] : x => Enum.GetNames<TEnum>()[x], relevance);
	}
}