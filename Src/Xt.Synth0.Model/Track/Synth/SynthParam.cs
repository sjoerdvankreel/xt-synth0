using System;

namespace Xt.Synth0.Model
{
	public sealed class SynthParam
	{
		public int Index { get; }
		public Param Param { get; }
		public IUIParamGroupModel Group { get; }
		internal SynthParam(IUIParamGroupModel owner, int index, Param param)
		{
			(Group, Index, Param) = (owner, index, param);
			if (index > 255) throw new InvalidOperationException();
		}
	}
}