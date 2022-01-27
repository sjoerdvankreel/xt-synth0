using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class ControlModel : IThemedModel
	{
		public string Name => "Control";
		public ThemeGroup Group => ThemeGroup.Control;
	}

	public sealed class MonitorModel : IThemedModel
	{
		public string Name => "Monitor";
		public ThemeGroup Group => ThemeGroup.Control;
	}

	public unsafe sealed class SeqModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal EditModel.Native edit;
			internal PatternModel.Native pattern;
		}

		public EditModel Edit { get; } = new();
		public PatternModel Pattern { get; } = new();
		public ControlModel Control { get; } = new();
		public MonitorModel Monitor { get; } = new();
		public override IReadOnlyList<ISubModel> SubModels => new[] { Edit };
		public override IReadOnlyList<IModelContainer> SubContainers => new[] { Pattern };
	}
}