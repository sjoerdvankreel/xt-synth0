using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public interface IThemedSubModel : ISubModel, IThemedModel { }
	public interface IThemedContainer : IModelContainer, IThemedModel { }
	public interface INativeModel { unsafe void* Address(void* parent); }
	public interface IThemedModel { string Name { get; } ThemeGroup Group { get; } }

	public interface IModelContainer : INativeModel
	{
		IReadOnlyList<ISubModel> SubModels { get; }
		IReadOnlyList<IModelContainer> SubContainers { get; }
	}

	public interface ISubModel : INativeModel
	{
		virtual int ColumnCount => 2;
		virtual IReadOnlyList<Param> Params 
		=> ParamLayout.Select(ps => ps.Key).ToArray();
		virtual IDictionary<Param, int> ParamLayout 
		=> Params.Select((p, i) => (p, i)).ToDictionary(e => e.p, e => e.i);
	}
}