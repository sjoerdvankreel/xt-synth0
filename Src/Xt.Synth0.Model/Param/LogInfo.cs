using System;

namespace Xt.Synth0.Model
{
	internal class LogInfo : ContinuousInfo
	{
		readonly int _range;
		readonly string _postfix1;
		readonly string _postfix1k;

		internal LogInfo(string name, int @default, int range, string postfix1, string postfix1k)
		: base(name, @default) => (_range, _postfix1, _postfix1k) = (range, postfix1, postfix1k);

		public override string Format(int value)
		{
			var max1 = Max + 1.0;
			var exp = (1.0 - Math.Log(max1 - value, max1)) * _range;
			if (exp < 1000) return $"{(int)exp}{_postfix1}";
			return $"{(exp / 1000).ToString("0.##")}{_postfix1k}";
		}
	}
}