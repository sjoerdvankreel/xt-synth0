namespace Xt.Synth0.Model
{
	public interface IGroupModel: IModel
	{
		Param[] Params();
	}

	public abstract class GroupModel<TModel> : Model<TModel>, IGroupModel
		where TModel : GroupModel<TModel>
	{
		public abstract Param[] Params();

		public override sealed void CopyTo(TModel model)
		{
			for (int i = 0; i < Params().Length; i++)
				model.Params()[i].Value = Params()[i].Value;
		}
	}
}