namespace Xt.Synth0.Model
{
	public interface IModel
	{
		void CopyTo(IModel model);
	}

	public abstract class Model<TModel> : IModel
		where TModel : Model<TModel>
	{
		public abstract void CopyTo(TModel model);
		public void CopyTo(IModel model) => CopyTo((TModel)model);
	}
}