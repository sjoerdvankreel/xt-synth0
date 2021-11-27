using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class GroupModel : SubModel
	{
		readonly Param[][] _paramGroups;
		public Param[][] ParamGroups() => _paramGroups;
		internal abstract Param[][] ListParamGroups();
		internal GroupModel() => _paramGroups = ListParamGroups();
		
		internal override sealed Param[] ListParams() 
		=> ListParamGroups().SelectMany(g => g).ToArray();
	}
}