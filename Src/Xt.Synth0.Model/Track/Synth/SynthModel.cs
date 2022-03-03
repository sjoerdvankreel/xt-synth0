using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum ModSource 
    { 
        Velo, 
        Env1, Env2, Env3, 
        LFO1, LFO2, LFO3 
    }

	public unsafe sealed class SynthModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public ref struct Native
		{
            internal CvModel cv;
            internal AmpModel.Native amp;
			internal PlotModel.Native plot;
            internal AudioModel audio;

			[StructLayout(LayoutKind.Sequential, Pack = 8)]
			public struct ParamInfo { public int min, max; }

			[StructLayout(LayoutKind.Sequential, Pack = 8)]
			public ref struct VoiceBinding { internal fixed byte @params[Model.ParamCount * 8]; }

			[StructLayout(LayoutKind.Sequential, Pack = 8)]
            internal ref struct CvModel
            {
				internal fixed byte lfos[Model.LfoCount * LfoModel.Native.Size];
				internal fixed byte envs[Model.EnvelopeCount * EnvModel.Native.Size];
			}

            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            internal ref struct AudioModel
            {
                internal fixed byte units[Model.UnitCount * UnitModel.Native.Size];
                internal fixed byte filts[Model.FilterCount * FilterModel.Native.Size];
            }

            [StructLayout(LayoutKind.Sequential, Pack = 8)]
            internal ref struct ModModel
            {
                internal int amount;
                internal int target;
                internal int source;
                internal int pad__;
            };
		}

		public IReadOnlyList<SynthParam> SynthParams { get; }

		public AmpModel Amp { get; } = new();
		public PlotModel Plot { get; } = new();
		public IReadOnlyList<LfoModel> Lfos = new ReadOnlyCollection<LfoModel>(MakeLfos());
		public IReadOnlyList<EnvModel> Envs = new ReadOnlyCollection<EnvModel>(MakeEnvs());
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());
		public IReadOnlyList<FilterModel> Filters = new ReadOnlyCollection<FilterModel>(MakeFilters());

		static IList<LfoModel> MakeLfos() => Enumerable.Range(0, Model.LfoCount).Select(i => new LfoModel(i)).ToList();
		static IList<EnvModel> MakeEnvs() => Enumerable.Range(0, Model.EnvelopeCount).Select(i => new EnvModel(i)).ToList();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, Model.UnitCount).Select(i => new UnitModel(i)).ToList();
		static IList<FilterModel> MakeFilters() => Enumerable.Range(0, Model.FilterCount).Select(i => new FilterModel(i)).ToList();

		public override int Index => 0;
		public override string Id => "8D6AB9FB-19DB-4F77-B56C-9E72AB67341F";
		public override IReadOnlyList<IGroupContainerModel> Children => new IGroupContainerModel[0];
		public override IReadOnlyList<IParamGroupModel> Groups => Units
			.Concat<IParamGroupModel>(Envs)
			.Concat(Lfos).Concat(Filters)
			.Concat(new IParamGroupModel[] { Plot, Amp }).ToArray();

		public SynthModel()
		{
			Envs[0].On.Value = 1;
			Units[0].On.Value = 1;
			var @params = ListParams(this).Select((p, i) => new SynthParam((IUIParamGroupModel)p.Group, i + 1, p.Param));
			SynthParams = new ReadOnlyCollection<SynthParam>(@params.ToArray());
			if (SynthParams.Count != Model.ParamCount)
				throw new InvalidOperationException();
		}

		public Native.ParamInfo[] ParamInfos()
		{
			var result = new Native.ParamInfo[Model.ParamCount];
			for (int i = 0; i < Model.ParamCount; i++)
			{
				result[i].min = Params[i].Info.Min;
				result[i].max = Params[i].Info.Max;
			}
			return result;
		}

		public void ToNative(Native.VoiceBinding* binding)
		{
			for (int i = 0; i < Params.Count; i++)
				*((int**)binding->@params)[i] = Params[i].Value;
		}

		public void FromNative(Native.VoiceBinding* binding)
		{
			for (int i = 0; i < Params.Count; i++)
				Params[i].Value = *((int**)binding->@params)[i];
		}

		public void BindVoice(Native* native, Native.VoiceBinding* binding)
		{
			var @params = (int**)binding->@params;
			for (int p = 0; p < Params.Count; p++)
				@params[p] = Params[p].Info.Address(SynthParams[p].Group.Address(native));
		}
	}
}