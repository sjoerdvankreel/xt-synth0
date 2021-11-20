namespace Xt.Synth0.Model
{
	public interface IGroupModel
	{
		Param<int>[] IntParams();
		Param<bool>[] BoolParams();
	}
}