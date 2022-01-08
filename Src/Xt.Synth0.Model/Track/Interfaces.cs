using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public interface INamedModel : ISubModel { string Name { get; } }
	public interface INativeModel { unsafe void* Address(void* parent); }

	public interface IModelGroup : INativeModel
	{
		IReadOnlyList<ISubModel> SubModels { get; }
		IReadOnlyList<IModelGroup> SubGroups { get; }
	}

	public interface ISubModel : INativeModel
	{
		virtual IReadOnlyList<Param> Params 
		=> ParamLayout.Select(ps => ps.Key).ToArray();
		virtual IDictionary<Param, int> ParamLayout 
		=> Params.Select((p, i) => (p, i)).ToDictionary(e => e.p, e => e.i);
	}
}