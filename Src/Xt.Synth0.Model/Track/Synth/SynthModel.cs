using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum SynthMethod { PBP, Add, Nve }

	public unsafe sealed class SynthModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			[StructLayout(LayoutKind.Sequential)]
			internal struct Param { internal int min, max; internal int* value; }

			internal AmpModel.Native amp;
			internal GlobalModel.Native global;
			internal fixed byte units[TrackConstants.UnitCount * TrackConstants.UnitSize];
			internal fixed byte @params[TrackConstants.ParamCount * TrackConstants.ParamSize];
		}

		public AmpModel Amp { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		public IReadOnlyList<AutoParam> AutoParams { get; }
		public override IReadOnlyList<IModelGroup> SubGroups => new IModelGroup[0];
		public AutoParam Auto(Param param) => AutoParams.Single(p => ReferenceEquals(param, p.Param));
		public override IReadOnlyList<ISubModel> SubModels => Units.Concat(new ISubModel[] { Amp, Global }).ToArray();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, TrackConstants.UnitCount).Select(i => new UnitModel(i)).ToList();

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
			Units[0].On.Value = 1;
			var @params = ListParams(this).Select((p, i) => new AutoParam((INamedModel)p.Model, i, p.Param));
			AutoParams = new ReadOnlyCollection<AutoParam>(@params.ToArray());
			if (AutoParams.Count != TrackConstants.ParamCount)
				throw new InvalidOperationException();
		}
	}
}