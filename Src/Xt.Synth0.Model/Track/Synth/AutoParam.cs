namespace Xt.Synth0.Model
{
	public sealed class AutoParam
	{
		public int Index { get; }
		public Param Param { get; }
		public IThemedSubModel Owner { get; }
		internal AutoParam(IThemedSubModel owner, int index, Param param) 
		=> (Owner, Index, Param) = (owner, index, param);
	}
}