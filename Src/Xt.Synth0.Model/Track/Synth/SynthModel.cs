using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum SyncStep
	{
		S0, S1_16, S1_8, S3_16, S1_4, S1_3, S3_8, S1_2, S5_8, S2_3, S3_4, S7_8, S15_16, S1_1, S9_8,
		S5_4, S4_3, S3_2, S5_3, S7_4, S15_8, S2_1, S3_1, S4_1, S5_1, S6_1, S7_1, S8_1, S10_1, S12_1, S16_1
	};

	public unsafe sealed class SynthModel : MainModel
	{
		static SynthModel()
		{
			if (Enum.GetValues<SyncStep>().Length != SyncStepNames.Length)
				throw new InvalidOperationException();
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			internal PlotModel.Native plot;
			internal GlobalModel.Native global;
			internal fixed byte lfos[Model.LfoCount * LfoModel.Native.Size];
			internal fixed byte envs[Model.EnvCount * EnvModel.Native.Size];
			internal fixed byte units[Model.UnitCount * UnitModel.Native.Size];
		}

		public PlotModel Plot { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<LfoModel> Lfos = new ReadOnlyCollection<LfoModel>(MakeLfos());
		public IReadOnlyList<EnvModel> Envs = new ReadOnlyCollection<EnvModel>(MakeEnvs());
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		static IList<LfoModel> MakeLfos() => Enumerable.Range(0, Model.LfoCount).Select(i => new LfoModel(i)).ToList();
		static IList<EnvModel> MakeEnvs() => Enumerable.Range(0, Model.EnvCount).Select(i => new EnvModel(i)).ToList();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, Model.UnitCount).Select(i => new UnitModel(i)).ToList();

		public IReadOnlyList<SynthParam> SynthParams { get; }
		public override IReadOnlyList<IModelContainer> SubContainers => new IModelContainer[0];
		public override IReadOnlyList<ISubModel> SubModels => Units.Concat<ISubModel>(Envs).Concat(Lfos).Concat(new ISubModel[] { Plot, Global }).ToArray();

		public static readonly string[] SyncStepNames = new[]
		{
			"0", "1/16", "1/8", "3/16", "1/4", "1/3", "3/8",
			"1/2", "5/8", "2/3", "3/4", "7/8", "15/16", "1/1",
			"9/8", "5/4", "4/3", "3/2", "5/3", "7/4", "15/8", "2/1",
			"3/1", "4/1", "5/1", "6/1", "7/1", "8/1", "10/1", "12/1", "16/1"
		};

		public SynthModel()
		{
			Envs[0].On.Value = 1;
			Units[0].On.Value = 1;
			var @params = ListParams(this).Select((p, i) => new SynthParam((IThemedSubModel)p.Sub, i + 1, p.Param));
			SynthParams = new ReadOnlyCollection<SynthParam>(@params.ToArray());
			if (SynthParams.Count != Model.ParamCount)
				throw new InvalidOperationException();
		}
	}
}