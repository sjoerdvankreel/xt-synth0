using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public unsafe abstract class MainModel : IModelGroup
	{
		public IReadOnlyList<Param> Params { get; }
		public abstract IReadOnlyList<ISubModel> SubModels { get; }
		public abstract IReadOnlyList<IModelGroup> SubGroups { get; }
		public event EventHandler<ParamChangedEventArgs> ParamChanged;
		public void* Address(void* parent) => throw new NotSupportedException();
		
		protected MainModel()
		{
			Params = new ReadOnlyCollection<Param>(ListParams(this).Select(p => p.Param).ToArray());
			for (int p = 0; p < Params.Count; p++)
			{
				var args = new ParamChangedEventArgs(p);
				Params[p].PropertyChanged += (s, e) => ParamChanged?.Invoke(this, args);
			}
		}

		public void CopyTo(MainModel model)
		{
			for (int p = 0; p < Params.Count; p++)
				model.Params[p].Value = Params[p].Value;
		}

		protected IList<(ISubModel Model, Param Param)> ListParams(IModelGroup group)
		{
			var result = new List<(ISubModel, Param)>();
			foreach (var model in group.SubModels)
				foreach (var param in model.Params)
					result.Add((model, param));
			foreach (var child in group.SubGroups)
				result.AddRange(ListParams(child));
			return result;
		}

		public void ToNative(IntPtr native) => ToNative(this, (void*)native);		
		void ToNative(IModelGroup group, void* native)
		{
			foreach (var model in group.SubModels)
			{
				var nativeSub = model.Address(native);
				foreach (var param in model.Params)
					*param.Info.Address(nativeSub) = param.Value;
			}
			foreach (var child in group.SubGroups)
				ToNative(child, child.Address(native));
		}

		public void FromNative(IntPtr native) => FromNative(this, (void*)native);
		void FromNative(IModelGroup group, void* native)
		{
			foreach (var model in group.SubModels)
			{
				var nativeSub = model.Address(native);
				foreach (var param in model.Params)
					param.Value = *param.Info.Address(nativeSub);
			}
			foreach (var child in group.SubGroups)
				FromNative(child, child.Address(native));
		}
	}
}