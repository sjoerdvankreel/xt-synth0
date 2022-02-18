using System;
using System.Linq;

namespace Xt.Synth0.Model
{
    unsafe delegate int* Address(void* parent);
    public enum ParamControl { Toggle, List, Knob };

    public sealed class ParamInfo
    {
        enum ParamType { Toggle, List, Lin, Mix, Time, Freq, Pattern };

        int? _maxDisplayLength;
        readonly Address _address;
        readonly Func<string, int> _load;
        readonly Func<int, string> _store;
        readonly Func<int, string> _display;

        ParamType Type { get; }
        public int Min { get; }
        public int Max { get; }
        public string Id { get; }
        public string Name { get; }
        public int Default { get; }
        public int SubGroup { get; }
        public string Description { get; }
        public IRelevance Relevance { get; }

        public ParamControl Control => Type switch
        {
            ParamType.Lin => ParamControl.Knob,
            ParamType.Mix => ParamControl.Knob,
            ParamType.Time => ParamControl.Knob,
            ParamType.Freq => ParamControl.Knob,
            ParamType.List => ParamControl.List,
            ParamType.Toggle => ParamControl.Toggle,
            _ => throw new InvalidOperationException()
        };

        public string Format(int value) => Type switch
        {
            ParamType.Lin => _display(value),
            ParamType.List => _display(value),
            ParamType.Time => FormatTime(value),
            ParamType.Freq => FormatFreq(value),
            ParamType.Pattern => _display(value),
            ParamType.Toggle => value == 0 ? "Off" : "On",
            ParamType.Mix => (value - 128).ToString("+#;-#;0"),
            _ => throw new InvalidOperationException()
        };

        string FormatTime(int value)
        {
            double ms = (value / 2.55) * (value / 2.55);
            if (ms < 1000.0) return $"{ms.ToString("N1")}m";
            if (ms < 10000.0) return $"{(ms / 1000.0).ToString("N1")}s";
            if (ms == 10000.0) return "10s";
            throw new InvalidOperationException();
        }

        string FormatFreq(int value)
        {
            double hz = 10.0 + 9990.0 * (value / 255.0) * (value / 255.0);
            if (hz < 1000.0) return $"{hz.ToString("N1")}h";
            if (hz < 10000.0) return $"{(hz / 1000.0).ToString("N1")}k";
            if (hz == 10000.0) return "10k";
            throw new InvalidOperationException();
        }

        public string Range(bool hex)
        {
            bool lin = Type != ParamType.Time;
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

        public bool IsMix => Type == ParamType.Mix;
        public unsafe int* Address(void* native) => _address(native);
        public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
        int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);

        ParamInfo(ParamType type, Address address, int subGroup, string id, string name, string description, int min, int max,
            int @default, Func<string, int> load, Func<int, string> store, Func<int, string> display, IRelevance relevance)
        {
            (Type, _address, SubGroup, Id, Name, Description, Min, Max, Default, _load, _store, _display, Relevance)
            = (type, address, subGroup, id, name, description, min, max, @default, load, store, display, relevance);
            if (subGroup < 0 || subGroup > 8) throw new InvalidOperationException();
            if (min < 0 || max > 255 || min >= max || @default < min || @default > max) throw new InvalidOperationException();
            _load ??= x => int.Parse(x);
            _store ??= x => x.ToString();
            _display ??= x => x.ToString();
        }

        internal static ParamInfo Pattern(
            Address address, string id, string name,
            string description, int min, int max, int @default)
         => new ParamInfo(ParamType.Pattern, address, 0, id, name,
             description, min, max, @default, null, null, null, null);

        internal static ParamInfo Freq(
            Address address, int subGroup, string id, string name,
            string description, int @default)
        => new ParamInfo(ParamType.Freq, address, subGroup, id, name,
            description, 0, 255, @default, null, null, null, null);

        internal static ParamInfo Mix(
            Address address, int subGroup, string id, string name,
            string description, IRelevance relevance = null)
        => new ParamInfo(ParamType.Mix, address, subGroup, id, name,
            description, 1, 255, 128, null, null, null, relevance);

        internal static ParamInfo Level(
            Address address, int subGroup, string id, string name,
            string description, int @default, IRelevance relevance = null)
        => new ParamInfo(ParamType.Lin, address, subGroup, id, name,
            description, 0, 255, @default, null, null, null, relevance);

        internal static ParamInfo Toggle(
            Address address, int subGroup, string id, string name,
            string description, bool @default, IRelevance relevance = null)
        => new ParamInfo(ParamType.Toggle, address, subGroup, id, name,
            description, 0, 1, @default ? 1 : 0, null, null, null, relevance);

        internal static ParamInfo Time(
            Address address, int subGroup, string id, string name,
            string description, int min, int @default, IRelevance relevance = null)
        => new ParamInfo(ParamType.Time, address, subGroup, id, name,
            description, min, 255, @default, null, null, null, relevance);

        internal static ParamInfo Pattern(
            Address address, string id, string name,
            string description, string[] display)
        => new ParamInfo(ParamType.Pattern, address, 0, id, name,
            description, 0, display.Length - 1, 0, null, null, x => display[x], null);

        internal static ParamInfo Select(
            Address address, int subGroup, string id, string name,
            string description, int min, int max, int @default, IRelevance relevance = null)
        => new ParamInfo(ParamType.Lin, address, subGroup, id, name,
            description, min, max, @default, null, null, null, relevance);

        internal static ParamInfo Select<TEnum>(
            Address address, int subGroup, string id, string name,
            string description, string[] display, IRelevance relevance = null) where TEnum : struct, Enum
        => new ParamInfo(ParamType.Lin, address, subGroup, id, name, description, 0,
            display.Length - 1, 0, LoadEnum<TEnum>, StoreEnum<TEnum>, x => display[x], relevance);

        internal static unsafe ParamInfo Step(
            Address address, int subGroup, string id, string name,
            string description, int min, int @default, IRelevance relevance = null)
        => new ParamInfo(ParamType.Lin, address, subGroup, id, name, description, min,
            SynthModel.SyncSteps.Length - 1, @default, null, null, val => SynthModel.SyncSteps[val].ToString(), relevance);

        internal static ParamInfo List<TEnum>(
            Address address, int subGroup, string id, string name,
            string description, string[] display = null, IRelevance relevance = null) where TEnum : struct, Enum
        => new ParamInfo(ParamType.List, address, subGroup, id, name, description, 0, Enum.GetValues<TEnum>().Length - 1,
            0, LoadEnum<TEnum>, StoreEnum<TEnum>, display != null ? x => display[x] : x => Enum.GetNames<TEnum>()[x], relevance);
    }
}