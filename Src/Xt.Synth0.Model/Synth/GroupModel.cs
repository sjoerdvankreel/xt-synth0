using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class IGroupModel
	{
		public abstract Param<int>[] IntParams();
		public abstract Param<bool>[] BoolParams();
		public IEnumerable<INotifyPropertyChanged> Params() 
		=> BoolParams().Cast<INotifyPropertyChanged>().Concat(IntParams());
	}
}