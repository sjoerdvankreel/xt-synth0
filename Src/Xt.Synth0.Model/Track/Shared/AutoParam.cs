using System;

namespace Xt.Synth0.Model
{
	public sealed class AutoParam
	{
		public int Index { get; }
		public Param Param { get; }
		public IUIParamGroupModel Group { get; }
		internal AutoParam(IUIParamGroupModel owner, int index, Param param)
		{
			(Group, Index, Param) = (owner, index, param);
			if (index > 255) throw new InvalidOperationException();
		}
	}
}