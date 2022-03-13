using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SequencerModel : MainModel
	{
        public override int Index => 0;
        public override string Id => "2468A725-5A55-4305-A438-C3A70DD3054F";
        public override IReadOnlyList<IParamGroupModel> Groups => new[] { Edit };
        public override IReadOnlyList<IGroupContainerModel> Children => new[] { Pattern };
        
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
	}
}