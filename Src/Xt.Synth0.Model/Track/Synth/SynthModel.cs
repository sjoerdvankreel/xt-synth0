using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SynthModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		public struct Native
		{
			[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
			internal struct Param { internal int min, max; internal int* value; }

			internal PlotModel.Native plot;
			internal GlobalModel.Native global;
			internal fixed byte units[TrackConstants.UnitCount * TrackConstants.UnitModelSize];
			internal fixed byte envs[TrackConstants.EnvCount * TrackConstants.EnvModelSize];
			internal fixed byte @params[TrackConstants.ParamCount * TrackConstants.ParamSize];
		}

		public PlotModel Plot { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<EnvModel> Envs = new ReadOnlyCollection<EnvModel>(MakeEnvs());
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		public IReadOnlyList<AutoParam> AutoParams { get; }
		public override IReadOnlyList<IModelContainer> SubContainers => new IModelContainer[0];
		public AutoParam Auto(Param param) => AutoParams.SingleOrDefault(p => ReferenceEquals(param, p.Param));
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, TrackConstants.UnitCount).Select(i => new UnitModel(i)).ToList();
		static IList<EnvModel> MakeEnvs() => Enumerable.Range(0, TrackConstants.EnvCount).Select(i => new EnvModel(i)).ToList();
		public override IReadOnlyList<ISubModel> SubModels => Units.Concat<ISubModel>(Envs).Concat(new ISubModel[] { Plot, Global }).ToArray();

		public void PrepareNative(IntPtr native)
		{
			Native* nativePtr = (Native*)native;
			var nativeParams = (Native.Param*)nativePtr->@params;
			for (int p = 0; p < Params.Count; p++)
			{
				nativeParams[p].min = AutoParams[p].Param.Info.Min;
				nativeParams[p].max = AutoParams[p].Param.Info.Max;
				var ownerAddress = AutoParams[p].Owner.Address(nativePtr);
				nativeParams[p].value = AutoParams[p].Param.Info.Address(ownerAddress);
			}
		}

		public SynthModel()
		{
			Units[0].Type.Value = 1;
			var @params = ListParams(this).Select((p, i) => new AutoParam((IThemedSubModel)p.Sub, i + 1, p.Param));
			AutoParams = new ReadOnlyCollection<AutoParam>(@params.ToArray());
			if (AutoParams.Count != TrackConstants.ParamCount)
				throw new InvalidOperationException();
		}
	}
}