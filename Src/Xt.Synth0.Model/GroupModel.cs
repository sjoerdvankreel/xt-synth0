using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public interface IGroupModel: IModel
	{
		Param<int>[] IntParams();
		Param<bool>[] BoolParams();
		IEnumerable<INotifyPropertyChanged> Params();
	}

	public abstract class GroupModel<TModel> : Model<TModel>, IGroupModel
		where TModel : GroupModel<TModel>
	{
		public abstract Param<int>[] IntParams();
		public abstract Param<bool>[] BoolParams();
		public IEnumerable<INotifyPropertyChanged> Params()
		=> BoolParams().Cast<INotifyPropertyChanged>().Concat(IntParams());

		public override sealed void CopyTo(TModel model)
		{
			for (int i = 0; i < IntParams().Length; i++)
				model.IntParams()[i].Value = IntParams()[i].Value;
			for (int i = 0; i < BoolParams().Length; i++)
				model.BoolParams()[i].Value = BoolParams()[i].Value;
		}
	}
}