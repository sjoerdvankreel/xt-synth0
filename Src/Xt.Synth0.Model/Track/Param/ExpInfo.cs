using System;

namespace Xt.Synth0.Model
{
	sealed class ExpInfo : DiscreteInfo
	{
		public override string Format(int value) => Math.Pow(2, value).ToString();
		internal ExpInfo(Address address, string name, string detail, int min, int max, int @default) :
		base(address, name, detail, min, max, @default)	{ }
	}
}