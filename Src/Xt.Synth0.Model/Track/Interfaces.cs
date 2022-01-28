using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public interface IUIModel
	{
		public string Name { get; }
		public ThemeGroup ThemeGroup { get; }
	}

	public interface IGroupContainerModel : INativeModel
	{
		IReadOnlyList<IParamGroupModel> Groups { get; }
		IReadOnlyList<IGroupContainerModel> Children { get; }
	}

	public interface IUIParamGroupModel : IParamGroupModel, IUIModel
	{
		public int Columns { get; }
		public abstract Param Enabled { get; }
		public abstract IDictionary<Param, int> Layout { get; }
	}

	public interface INativeModel { unsafe void* Address(void* parent); }
	public interface IUIGroupContainerModel : IGroupContainerModel, IUIModel { }
	public interface IParamGroupModel : INativeModel { IReadOnlyList<Param> Params { get; } }
}