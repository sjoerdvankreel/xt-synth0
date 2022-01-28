using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public unsafe abstract class MainModel : IGroupContainerModel
	{
		public IReadOnlyList<Param> Params { get; }
		public event EventHandler<ParamChangedEventArgs> ParamChanged;

		public abstract IReadOnlyList<IParamGroupModel> Groups { get; }
		public abstract IReadOnlyList<IGroupContainerModel> Children { get; }
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

		public void FromNative(void* native) => FromNative(this, native);
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

		protected IList<(IParamGroupModel Group, Param Param)> ListParams(IGroupContainerModel container)
		{
			var result = new List<(IParamGroupModel, Param)>();
			for (int i = 0; i < container.Groups.Count; i++)
				for (int j = 0; j < container.Groups[i].Params.Count; j++)
					result.Add((container.Groups[i], container.Groups[i].Params[j]));
			for (int i = 0; i < container.Children.Count; i++)
				result.AddRange(ListParams(container.Children[i]));
			return result;
		}
	}
}