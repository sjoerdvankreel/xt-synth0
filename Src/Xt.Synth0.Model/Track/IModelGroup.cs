namespace Xt.Synth0.Model
{
	public interface IModelGroup : INativeModel
	{
		ISubModel[] SubModels { get; }
	}
}