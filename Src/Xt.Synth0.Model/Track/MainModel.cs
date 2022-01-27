using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public unsafe abstract class MainModel : IModelContainer
	{
		public IReadOnlyList<Param> Params { get; }
		public abstract IReadOnlyList<ISubModel> SubModels { get; }
		public abstract IReadOnlyList<IModelContainer> SubContainers { get; }
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

		public void CopyTo(MainModel main)
		{
			for (int p = 0; p < Params.Count; p++)
				main.Params[p].Value = Params[p].Value;
		}

		public void ToNative(void* native) => ToNative(this, native);
		void ToNative(IModelContainer container, void* native)
		{
			for (int i = 0; i < container.SubContainers.Count; i++)
			{
				var child = container.SubContainers[i];
				ToNative(child, child.Address(native));
			}
			for (int i = 0; i < container.SubModels.Count; i++)
			{
				var child = container.SubModels[i];
				var nativeSub = child.Address(native);
				for (int j = 0; j < child.Params.Count; j++)
				{
					var childParam = child.Params[j];
					*childParam.Info.Address(nativeSub) = childParam.Value;
				}
			}
		}

		public void FromNative(void* native) => FromNative(this, native);
		void FromNative(IModelContainer container, void* native)
		{
			for (int i = 0; i < container.SubContainers.Count; i++)
			{
				var child = container.SubContainers[i];
				FromNative(child, child.Address(native));
			}
			for (int i = 0; i < container.SubModels.Count; i++)
			{
				var child = container.SubModels[i];
				var nativeSub = child.Address(native);
				for (int j = 0; j < child.Params.Count; j++)
				{
					var childParam = child.Params[j];
					childParam.Value = *childParam.Info.Address(nativeSub);
				}
			}
		}

		protected IList<(ISubModel Sub, Param Param)> ListParams(IModelContainer container)
		{
			var result = new List<(ISubModel, Param)>();
			foreach (var model in container.SubModels)
				foreach (var param in model.Params)
					result.Add((model, param));
			foreach (var child in container.SubContainers)
				result.AddRange(ListParams(child));
			return result;
		}
	}
}