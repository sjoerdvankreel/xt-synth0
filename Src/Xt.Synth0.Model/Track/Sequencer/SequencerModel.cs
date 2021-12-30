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
			internal EditModel.Native edit;
			internal PatternModel.Native pattern;
		}

		public EditModel Edit { get; } = new();
		public PatternModel Pattern { get; } = new();

		public event EventHandler ParamChanged;
		internal SequencerModel() => BindParamChanged(this);
		public IReadOnlyList<ISubModel> SubModels => new[] { Edit };
		public IReadOnlyList<IModelGroup> SubGroups => new[] { Pattern };
		public void* Address(void* parent) => throw new NotSupportedException();

		void BindParamChanged(IModelGroup group)
		{
			foreach (var model in group.SubModels)
				BindParamChanged(model);
			foreach (var child in group.SubGroups)
				BindParamChanged(child);
		}

		void BindParamChanged(ISubModel model)
		{
			foreach (var param in model.Params)
				param.PropertyChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}