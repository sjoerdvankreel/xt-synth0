using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum SynthMethod { PBP, Add, Nve }

	public unsafe sealed class SynthModel : IModelGroup
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			[StructLayout(LayoutKind.Sequential)]
			internal struct Param { int min, max; int* value; }

			internal AmpModel.Native amp;
			internal GlobalModel.Native global;
			internal fixed byte units[TrackConstants.UnitCount * TrackConstants.UnitSize];
			internal fixed byte @params[TrackConstants.ParamCount * TrackConstants.ParamSize];
		}

		IList<(ISubModel Owner, Param Param)> ListParams(IModelGroup group)
		{
			var result = new List<(ISubModel, Param)>();
			foreach (var model in group.SubModels)
				foreach (var param in model.Params)
					result.Add((model, param));
			foreach (var child in group.SubGroups)
				result.AddRange(ListParams(child));
			return result;
		}

		public AmpModel Amp { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		public IReadOnlyList<AutoParam> Params { get; }
		public event EventHandler<ParamChangedEventArgs> ParamChanged;
		public IReadOnlyList<IModelGroup> SubGroups => new IModelGroup[0];
		public void* Address(void* parent) => throw new NotSupportedException();
		public AutoParam Auto(Param param) => Params.Single(p => ReferenceEquals(param, p.Param));
		public IReadOnlyList<ISubModel> SubModels => Units.Concat(new ISubModel[] { Amp, Global }).ToArray();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, TrackConstants.UnitCount).Select(i => new UnitModel(i)).ToList();

		public SynthModel()
		{
			Units[0].On.Value = 1;
			var @params = ListParams(this);
			if (@params.Count != TrackConstants.ParamCount)
				throw new InvalidOperationException();
			var autoParams = new List<AutoParam>();
			for (int i = 0; i < @params.Count; i++)
				autoParams.Add(new AutoParam((INamedModel)@params[i].Owner, i, @params[i].Param));
			Params = new ReadOnlyCollection<AutoParam>(autoParams);
			for (int p = 0; p < Params.Count; p++)
			{
				int local = p;
				Params[p].Param.PropertyChanged += (s, e) => ParamChanged?.Invoke(this, new ParamChangedEventArgs(local));
			}
		}
	}
}