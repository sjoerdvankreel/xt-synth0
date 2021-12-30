using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SequencerModel : IModelGroup
	{
		static SequencerModel()
		{
			if (Size != XtsSequencerModelSize())
				throw new InvalidOperationException();
		}

		internal const int Size = 1;
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsSequencerModelSize();
		[StructLayout(LayoutKind.Sequential)]
		internal unsafe struct Native
		{
			EditModel.Native edit;
			PatternModel.Native pattern;
		}

		public EditModel Edit { get; } = new();
		public PatternModel Pattern { get; } = new();

		public IReadOnlyList<ISubModel> SubModels => new[] { Edit };
		public IReadOnlyList<IModelGroup> SubGroups => new[] { Pattern };
		public void* Address(void* parent) => throw new NotSupportedException();
	}
}