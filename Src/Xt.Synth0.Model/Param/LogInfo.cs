using System;

namespace Xt.Synth0.Model
{
	class LogInfo : ContinuousInfo
	{
		readonly int _range;
		readonly string _postfix1;
		readonly string _postfix1k;

		internal LogInfo(string name, string detail, int @default, 
			int range, string postfix1, string postfix1k)
		: base(name, detail, @default) 
		=> (_range, _postfix1, _postfix1k) = (range, postfix1, postfix1k);

		public override string Format(int value)
		{
			var pos = (double)value / Max * 9.0;
			var log = (1.0 - Math.Log10(10.0 - pos)) * _range;
			if (log < 1000) return $"{(int)log}{_postfix1}";
			return $"{(log / 1000).ToString("0.##")}{_postfix1k}";
		}
	}
}