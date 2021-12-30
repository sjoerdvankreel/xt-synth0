using System;

namespace Xt.Synth0.Model
{
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
}