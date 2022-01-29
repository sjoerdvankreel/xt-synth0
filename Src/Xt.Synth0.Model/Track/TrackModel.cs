using System;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class TrackModel : IGroupContainerModel
	{
		public event EventHandler ParamChanged;
		public SeqModel Seq { get; } = new();
		public SynthModel Synth { get; } = new();

		public int Index => 0;
		public string Id => "5346DB88-3727-4DAE-B61D-4D1BDACDDE61";
		public IReadOnlyList<IParamGroupModel> Groups => new IParamGroupModel[0];
		public unsafe void* Address(void* parent) => throw new NotSupportedException();
		public IReadOnlyList<IGroupContainerModel> Children => new IGroupContainerModel[] { Seq, Synth };

		public TrackModel()
		{
			Seq.ParamChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			Synth.ParamChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
		}

		public void CopyTo(TrackModel track)
		{
			Seq.CopyTo(track.Seq);
			Synth.CopyTo(track.Synth);
		}

		public StoreModel Store() => Store(this);
		StoreModel Store(IParamGroupModel group)
		{
			var result = new StoreModel();
			foreach (var param in group.Params)
				result.Params.Add(param.Info.Id, param.Value);
			return result;
		}

		StoreModel Store(IGroupContainerModel model)
		{
			var result = new StoreModel();
			foreach (var child in model.Children)
				result.Children.Add(new StoreKey(child.Id, child.Index), Store(child));
			foreach (var group in model.Groups)
				result.Children.Add(new StoreKey(group.Id, group.Index), Store(group));
			return result;
		}

		public static TrackModel Load(StoreModel store)
		{
			var result = new TrackModel();
			Load(result, store);
			return result;
		}

		static void Load(IParamGroupModel model, StoreModel store)
		{
			foreach (var entry in store.Params)
			{
				var param = model.Params.SingleOrDefault(
					p => p.Info.Id == entry.Key);
				if (param != null)
					param.Value = entry.Value;
			}
		}

		static void Load(IGroupContainerModel model, StoreModel store)
		{
			foreach (var entry in store.Children)
			{
				var child = model.Children.SingleOrDefault(c =>
					c.Id == entry.Key.Id && c.Index == entry.Key.Index);
				if (child != null)
					Load(child, entry.Value);
				var group = model.Groups.SingleOrDefault(g =>
					g.Id == entry.Key.Id && g.Index == entry.Key.Index);
				if (group != null)
					Load(group, entry.Value);
			}
		}
	}
}