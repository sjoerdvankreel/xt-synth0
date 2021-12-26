using System;

namespace Xt.Synth0.Model
{
	sealed class ExpInfo : DiscreteInfo
	{
		internal ExpInfo(string name, string detail, int min, int max, int @default)
		: base(name, detail, min, max, @default) { }
		public override string Format(int value) => Math.Pow(2, value).ToString();
	}
}