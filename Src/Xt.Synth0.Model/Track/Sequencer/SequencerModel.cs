using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SequencerModel : IModelGroup
	{
		[StructLayout(LayoutKind.Sequential)]
		internal unsafe struct Native
		{
			internal EditModel.Native edit;
			internal PatternModel.Native pattern;
		}

		public EditModel Edit { get; } = new();
		public PatternModel Pattern { get; } = new();

		public event EventHandler ParamChanged;
		public IReadOnlyList<Param> Params { get; }
		public IReadOnlyList<ISubModel> SubModels => new[] { Edit };
		public IReadOnlyList<IModelGroup> SubGroups => new[] { Pattern };
		public void* Address(void* parent) => throw new NotSupportedException();

		IList<Param> ListParams(IModelGroup group)
		{
			var result = new List<Param>();
			foreach (var model in group.SubModels)
				foreach (var param in model.Params)
					result.Add(param);
			foreach (var child in group.SubGroups)
				result.AddRange(ListParams(child));
			return result;
		}

		public void CopyTo(SequencerModel model)
		{
			for (int p = 0; p < Params.Count; p++)
				model.Params[p].Value = Params[p].Value;
		}

		public SequencerModel()
		{
			Params = new ReadOnlyCollection<Param>(ListParams(this));
			for (int p = 0; p < Params.Count; p++)
				Params[p].PropertyChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}