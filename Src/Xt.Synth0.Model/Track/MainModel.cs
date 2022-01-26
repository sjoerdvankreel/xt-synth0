using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public unsafe abstract class MainModel<TNative, TStored> : IModelContainer, IStoredModel<TNative, TStored>
	where TNative : struct
	where TStored : struct
	{
		public abstract IReadOnlyList<ISubModel> SubModels { get; }
		public abstract IReadOnlyList<IModelContainer> SubContainers { get; }
		public abstract void Load(in TStored stored, out TNative native);
		public abstract void Store(in TNative native, out TStored stored);

		public IReadOnlyList<Param> Params { get; }
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

		public void CopyTo(MainModel<TNative, TStored> main)
		{
			for (int p = 0; p < Params.Count; p++)
				main.Params[p].Value = Params[p].Value;
		}

		public void ToNative(void* native) => ToNative(this, native);
		void ToNative(IModelContainer container, void* native)
		{
			foreach (var model in container.SubModels)
			{
				var nativeSub = model.Address(native);
				foreach (var param in model.Params)
					*param.Info.Address(nativeSub) = param.Value;
			}
			foreach (var child in container.SubContainers)
				ToNative(child, child.Address(native));
		}

		public void FromNative(void* native) => FromNative(this, native);
		void FromNative(IModelContainer container, void* native)
		{
			foreach (var model in container.SubModels)
			{
				var nativeSub = model.Address(native);
				foreach (var param in model.Params)
					param.Value = *param.Info.Address(nativeSub);
			}
			foreach (var child in container.SubContainers)
				FromNative(child, child.Address(native));
		}

		protected IList<(ISubModel Sub, Param Param)> ListParams(IModelContainer containers)
		{
			var result = new List<(ISubModel, Param)>();
			foreach (var model in containers.SubModels)
				foreach (var param in model.Params)
					result.Add((model, param));
			foreach (var child in containers.SubContainers)
				result.AddRange(ListParams(child));
			return result;
		}
	}
}