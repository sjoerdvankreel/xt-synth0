﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SynthModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public ref struct Native
        {
            internal PlotModel.Native plot;
            internal VoiceModel.Native voice;
            internal LfoModel.Native globalLfo;
        }

		public IReadOnlyList<SynthParam> SynthParams { get; }

		public AmpModel Amp { get; } = new();
		public PlotModel Plot { get; } = new();
        public LfoModel GlobalLfo { get; } = new(SynthConfig.VoiceLfoCount + 1);
		public IReadOnlyList<LfoModel> Lfos = new ReadOnlyCollection<LfoModel>(MakeLfos());
		public IReadOnlyList<EnvModel> Envs = new ReadOnlyCollection<EnvModel>(MakeEnvs());
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());
		public IReadOnlyList<FilterModel> Filters = new ReadOnlyCollection<FilterModel>(MakeFilters());

		static IList<LfoModel> MakeLfos() => Enumerable.Range(0, SynthConfig.VoiceLfoCount).Select(i => new LfoModel(i)).ToList();
		static IList<EnvModel> MakeEnvs() => Enumerable.Range(0, SynthConfig.VoiceEnvCount).Select(i => new EnvModel(i)).ToList();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, SynthConfig.VoiceUnitCount).Select(i => new UnitModel(i)).ToList();
		static IList<FilterModel> MakeFilters() => Enumerable.Range(0, SynthConfig.VoiceFilterCount).Select(i => new FilterModel(i)).ToList();

		public override int Index => 0;
		public override string Id => "8D6AB9FB-19DB-4F77-B56C-9E72AB67341F";
		public override IReadOnlyList<IGroupContainerModel> Children => new IGroupContainerModel[0];
		public override IReadOnlyList<IParamGroupModel> Groups => Units
			.Concat<IParamGroupModel>(Envs)
			.Concat(Lfos).Concat(Filters)
			.Concat(new IParamGroupModel[] { Plot, Amp/*, GlobalLfo*/ }).ToArray();

		public SynthModel()
		{
			Envs[0].On.Value = 1;
			Units[0].On.Value = 1;
			var @params = ListParams(this).Select((p, i) => new SynthParam((IUIParamGroupModel)p.Group, i + 1, p.Param));
			SynthParams = new ReadOnlyCollection<SynthParam>(@params.ToArray());
			if (SynthParams.Count != SynthConfig.SynthParamCount)
				throw new InvalidOperationException();
		}

		public void ToNative(ParamBinding.Native* binding)
		{
			for (int i = 0; i < Params.Count; i++)
				*((int**)binding->@params)[i] = Params[i].Value;
		}

		public void FromNative(ParamBinding.Native* binding)
		{
			for (int i = 0; i < Params.Count; i++)
				Params[i].Value = *((int**)binding->@params)[i];
        }

        public ParamInfo.Native[] ParamInfos()
        {
            var result = new ParamInfo.Native[SynthConfig.SynthParamCount];
            for (int i = 0; i < SynthConfig.SynthParamCount; i++)
            {
                result[i].min = Params[i].Info.Min;
                result[i].max = Params[i].Info.Max;
            }
            return result;
        }

        public void BindVoice(Native* native, ParamBinding.Native* binding)
		{
			var @params = binding->@params;
			for (int p = 0; p < Params.Count; p++)
				@params[p] = Params[p].Info.Address(SynthParams[p].Group.Address(native));
		}
	}
}