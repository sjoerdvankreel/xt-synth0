﻿using System;

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

	class ContinuousInfo : ParamInfo
	{
		readonly int _default;

		public override int Min => 0;
		public override int Max => 255;
		public override bool IsToggle => false;
		public override int Default => _default;

		public override string Format(int value)
		=> (((double)value - Min) / (Max - Min)).ToString("P1");
		internal ContinuousInfo(Address address, string name, string detail, int @default) :
		base(address, name, detail) => _default = @default;
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

	sealed class LogInfo : ContinuousInfo
	{
		readonly int _range;
		readonly string _postfix1;
		readonly string _postfix1k;

		public override string Format(int value)
		{
			var pos = (double)value / Max * 9.0;
			var log = (1.0 - Math.Log10(10.0 - pos)) * _range;
			if (log < 1000) return $"{(int)log}{_postfix1}";
			return $"{(log / 1000).ToString("0.##")}{_postfix1k}";
		}

		internal LogInfo(Address address, string name, string detail, int @default, int range, string postfix1, string postfix1k) :
		base(address, name, detail, @default) => (_range, _postfix1, _postfix1k) = (range, postfix1, postfix1k);
	}
}