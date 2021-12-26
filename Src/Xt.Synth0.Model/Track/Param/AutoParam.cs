namespace Xt.Synth0.Model
{
	public sealed class AutoParam
	{
		public int Index { get; }
		public Param Param { get; }
		public GroupModel Owner { get; }

		internal AutoParam(GroupModel owner, Param param, int index)
		=> (Owner, Param, Index) = (owner, param, index);
	}
}