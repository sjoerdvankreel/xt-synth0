using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class ControlModel : IUIModel
	{
		public string Name => "Control";
		public ThemeGroup ThemeGroup => ThemeGroup.Control;
	}

	public sealed class MonitorModel : IUIModel
	{
		public string Name => "Monitor";
		public ThemeGroup ThemeGroup => ThemeGroup.Control;
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

		public override IReadOnlyList<IParamGroupModel> Groups => new[] { Edit };
		public override IReadOnlyList<IGroupContainerModel> Children => new[] { Pattern };
	}
}