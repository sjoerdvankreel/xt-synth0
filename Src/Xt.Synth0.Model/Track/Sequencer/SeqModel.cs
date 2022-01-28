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
		public ref struct Native
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

		public void ToNative(Native* native) => ToNative(this, native);
		void ToNative(IGroupContainerModel container, void* native)
		{
			for (int i = 0; i < container.Children.Count; i++)
				ToNative(container.Children[i], container.Children[i].Address(native));
			for (int i = 0; i < container.Groups.Count; i++)
				for (int j = 0; j < container.Groups[i].Params.Count; j++)
				{
					var param = container.Groups[i].Params[j];
					*param.Info.Address(container.Groups[i].Address(native)) = param.Value;
				}
		}

		public void FromNative(Native* native) => FromNative(this, native);
		void FromNative(IGroupContainerModel container, void* native)
		{
			for (int i = 0; i < container.Children.Count; i++)
				FromNative(container.Children[i], container.Children[i].Address(native));
			for (int i = 0; i < container.Groups.Count; i++)
				for (int j = 0; j < container.Groups[i].Params.Count; j++)
				{
					var param = container.Groups[i].Params[j];
					param.Value = *param.Info.Address(container.Groups[i].Address(native));
				}
		}
	}
}