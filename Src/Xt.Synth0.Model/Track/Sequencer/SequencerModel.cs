using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class SequencerModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal EditModel.Native edit;
			internal PatternModel.Native pattern;
		}

		public EditModel Edit { get; } = new();
		public PatternModel Pattern { get; } = new();
		public override IReadOnlyList<ISubModel> SubModels => new[] { Edit };
		public override IReadOnlyList<IModelGroup> SubGroups => new[] { Pattern };
	}
}