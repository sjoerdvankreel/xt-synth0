using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public interface INamedModel : ISubModel { string Name { get; } }
	public interface INativeModel { unsafe void* Address(void* parent); }
	public interface ISubModel : INativeModel { IReadOnlyList<Param> Params { get; } }
	
	public interface IModelGroup : INativeModel 
	{
		IReadOnlyList<ISubModel> SubModels { get; }
		IReadOnlyList<IModelGroup> SubGroups { get; }
	}
}