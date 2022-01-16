using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum AmpEnv { NoAmpEnv, AmpEnv1, AmpEnv2 }
	public enum SyncStep
	{
		Step1_16, Step1_9, Step1_8, Step3_16, Step2_9, Step1_4, Step5_16, Step1_3, Step3_8, Step7_16, Step4_9, Step1_2,
		Step5_9, Step9_16, Step5_8, Step2_3, Step11_16, Step3_4, Step7_9, Step13_16, Step7_8, Step8_9, Step15_16, Step1_1,
		Step1_1_16, Step1_1_9, Step1_1_8, Step1_3_16, Step1_2_9, Step1_1_4, Step1_5_16, Step1_1_3, Step1_3_8, Step1_7_16, Step1_4_9,
		Step1_1_2, Step1_5_9, Step1_9_16, Step1_5_8, Step1_2_3, Step1_11_16, Step1_3_4, Step1_7_9, Step1_13_16, Step1_7_8, Step1_8_9, Step1_15_16,
		Step2_1, Step3_1, Step4_1, Step5_1, Step6_1, Step7_1, Step8_1, Step9_1, Step10_1, Step11_1, Step12_1, Step13_1, Step14_1, Step15_1, Step16_1
	};

	public unsafe sealed class SynthModel : MainModel
	{
		static SynthModel()
		{
			if (Enum.GetValues<SyncStep>().Length != SyncStepNames.Length)
				throw new InvalidOperationException();
		}

		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		public struct Native
		{
			[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
			internal struct AutoParam { internal int min, max; internal int* value; }

			internal PlotModel.Native plot;
			internal GlobalModel.Native global;
			internal fixed byte units[TrackConstants.UnitCount * TrackConstants.UnitModelSize];
			internal fixed byte envs[TrackConstants.EnvCount * TrackConstants.EnvModelSize];
			internal fixed byte autoParams[TrackConstants.AutoParamCount * TrackConstants.AutoParamSize];
		}

		public PlotModel Plot { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<EnvModel> Envs = new ReadOnlyCollection<EnvModel>(MakeEnvs());
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		public IReadOnlyList<AutoParam> AutoParams { get; }
		public override IReadOnlyList<IModelContainer> SubContainers => new IModelContainer[0];
		public AutoParam AutoParam(Param param) => AutoParams.Single(p => ReferenceEquals(param, p.Param));
		static IList<EnvModel> MakeEnvs() => Enumerable.Range(0, TrackConstants.EnvCount).Select(i => new EnvModel(i)).ToList();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, TrackConstants.UnitCount).Select(i => new UnitModel(i)).ToList();
		public override IReadOnlyList<ISubModel> SubModels => Units.Concat<ISubModel>(Envs).Concat(new ISubModel[] { Plot, Global }).ToArray();

		public static readonly string[] SyncStepNames = new[]
		{
			"1/16", "1/9", "1/8", "3/16", "2/9", "1/4", "5/16", "1/3", "3/8", "7/16", "4/9", "1/2",
			"5/9", "9/16", "5/8", "2/3", "11/16", "3/4", "7/9", "13/16", "7/8", "8/9", "15/16", "1/1",
			"1 1/16", "1 1/9", "1 1/8", "1 3/16", "1 2/9", "1 1/4", "1 5/16", "1 1/3", "1 3/8", "1 7/16", "1 4/9", "1 1/2",
			"1 5/9", "1 9/16", "1 5/8", "1 2/3", "1 11/16", "1 3/4", "1 7/9", "1 13/16", "1 7/8", "1 8/9", "1 15/16",
			"2/1", "3/1", "4/1", "5/1", "6/1", "7/1", "8/1", "9/1", "10/1", "11/1", "12/1", "13/1", "14/1", "15/1", "16/1"
		};

		public SynthModel()
		{
			Units[0].Type.Value = (int)UnitType.Sin;
			Envs[0].Type.Value = (int)EnvType.DAHDSR;
			Global.AmpEnv.Value = (int)AmpEnv.AmpEnv1;
			var @params = ListParams(this).Where(p => p.Param.Info.Automatable)
				.Select((p, i) => new AutoParam((IThemedSubModel)p.Sub, i + 1, p.Param));
			AutoParams = new ReadOnlyCollection<AutoParam>(@params.ToArray());
			if (AutoParams.Count != TrackConstants.AutoParamCount)
				throw new InvalidOperationException();
		}

		public void PrepareNative(IntPtr native)
		{
			Native* nativePtr = (Native*)native;
			var nativeParams = (Native.AutoParam*)nativePtr->autoParams;
			for (int p = 0; p < AutoParams.Count; p++)
			{
				nativeParams[p].min = AutoParams[p].Param.Info.Min;
				nativeParams[p].max = AutoParams[p].Param.Info.Max;
				var ownerAddress = AutoParams[p].Owner.Address(nativePtr);
				nativeParams[p].value = AutoParams[p].Param.Info.Address(ownerAddress);
			}
		}
	}
}