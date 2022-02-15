using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public interface IUIModel
	{
		string Name { get; }
		ThemeGroup ThemeGroup { get; }
	}

	public interface IStoredModel
	{
		int Index { get; }
		string Id { get; }
		unsafe void* Address(void* parent);
	}

	public interface IGroupContainerModel : IStoredModel
	{
		IReadOnlyList<IParamGroupModel> Groups { get; }
		IReadOnlyList<IGroupContainerModel> Children { get; }
	}

	public interface IUIParamGroupModel : IParamGroupModel, IUIModel
	{
		int Columns { get; }
		string[] In { get; }
		string[] Out { get; }
		Param Enabled { get; }
		IDictionary<Param, int> Layout { get; }
	}

	public interface IUIGroupContainerModel : IGroupContainerModel, IUIModel { }
	public interface IParamGroupModel : IStoredModel { IReadOnlyList<Param> Params { get; } }
}