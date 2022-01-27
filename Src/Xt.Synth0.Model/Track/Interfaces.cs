using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public interface IThemedSubModel : ISubModel, IThemedModel { }
	public interface IThemedContainer : IModelContainer, IThemedModel { }
	public interface INativeModel { unsafe void* Address(void* parent); }
	public interface IThemedModel { string Name { get; } ThemeGroup Group { get; } }

	public interface IStoredModel<TNative, TStored>: INativeModel
	where TNative : struct
	where TStored : struct
	{
		void Load(ref TStored stored, ref TNative native);
		void Store(ref TNative native, ref TStored stored);
	}

	public interface IModelContainer : INativeModel
	{
		IReadOnlyList<ISubModel> SubModels { get; }
		IReadOnlyList<IModelContainer> SubContainers { get; }
	}

	public interface ISubModel : INativeModel
	{
		virtual int ColumnCount => 3;
		virtual Param Enabled => null;
		virtual IReadOnlyList<Param> Params
		=> ParamLayout.Select(ps => ps.Key).ToArray();
		virtual IDictionary<Param, int> ParamLayout
		=> Params.Select((p, i) => (p, i)).ToDictionary(e => e.p, e => e.i);
	}
}