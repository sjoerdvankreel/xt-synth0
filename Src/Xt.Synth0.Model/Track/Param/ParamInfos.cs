using System;

namespace Xt.Synth0.Model
{
	sealed class ToggleInfo : ParamInfo
	{
		public override int Min => 0;
		public override int Max => 1;
		public override int Default => 0;
		public override bool IsToggle => true;

		internal ToggleInfo(Address address, string name, string detail) :
		base(address, name, detail)	{ }
		public override string Format(int value) => value == 0 ? "Off" : "On";
	}

	sealed class EnumInfo<T> : ParamInfo
	   where T : struct, Enum
	{
		readonly string[] _names;

		public override int Min => 0;
		public override int Default => 0;
		public override bool IsToggle => false;
		public override int Max => Enum.GetValues<T>().Length - 1;

		public override string Format(int value) => _names[value];
		internal EnumInfo(Address address, string name, string detail, string[] names) :
		base(address, name, detail) => _names = names;
	}

	sealed class ExpInfo : DiscreteInfo
	{
		public override string Format(int value) => Math.Pow(2, value).ToString();
		internal ExpInfo(Address address, string name, string detail, int min, int max, int @default) :
		base(address, name, detail, min, max, @default)	{ }
	}

	class DiscreteInfo : ParamInfo
	{
		readonly int _min;
		readonly int _max;
		readonly int _default;

		public override int Min => _min;
		public override int Max => _max;
		public override bool IsToggle => false;
		public override int Default => _default;

		public override string Format(int value) => value.ToString();
		internal DiscreteInfo(Address address, string name, string detail, int min, int max, int @default) :
		base(address, name, detail) => (_min, _max, _default) = (min, max, @default);
	}
}