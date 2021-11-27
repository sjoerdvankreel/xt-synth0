namespace Xt.Synth0.Model
{
	public class AutoParam
	{
		public Param Param { get; }
		public string Group { get; }

		internal AutoParam(string group, Param param)
		=> (Group, Param) = (group, param);
	}
}