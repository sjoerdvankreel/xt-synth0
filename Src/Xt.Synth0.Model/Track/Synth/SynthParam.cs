namespace Xt.Synth0.Model
{
	public sealed class SynthParam
	{
		public int Index { get; }
		public Param Param { get; }
		public IThemedSubModel Owner { get; }
		internal SynthParam(IThemedSubModel owner, int index, Param param) 
		=> (Owner, Index, Param) = (owner, index, param);
	}
}