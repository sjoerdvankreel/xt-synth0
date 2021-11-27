using System;

namespace Xt.Synth0.Model
{
	internal sealed class EnumInfo<T> : ParamInfo
		where T : struct, Enum
	{
		public override int Min => 0;
		public override int Default => 0;
		public override bool IsToggle => false;
		public override int Max => Enum.GetValues<T>().Length - 1;

		readonly string[] _names;
		public override string Format(int value) => _names[value];
		internal EnumInfo(string name, string[] names) : base(name) => _names = names;
	}
}