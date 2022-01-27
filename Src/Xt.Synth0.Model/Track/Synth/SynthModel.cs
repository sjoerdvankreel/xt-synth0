using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SynthModel : MainModel
	{
		static int GCD(int a, int b)
		{
			while (b > 0) { int rem = a % b; a = b; b = rem; }
			return a;
		}

		static readonly int[] BaseSteps = { 1, 2, 3, 4, 6, 9, 16 };
		static IEnumerable<SyncStep> Cartesian()
		=> BaseSteps.SelectMany(n => BaseSteps.Select(d => new SyncStep { num = n, den = d }));
		static IEnumerable<SyncStep> AllSteps() => Cartesian()
		.Concat(new[] { new SyncStep { num = 0, den = 1 } })
		.Concat(Cartesian().Where(s => s.val < 1.0f).Select(s => new SyncStep { num = s.num + s.den, den = s.den }));
		public static readonly SyncStep[] SyncSteps = AllSteps().Select(s => s.Simplify()).Distinct().OrderBy(s => s.val).ToArray();

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal PlotModel.Native plot;
			internal GlobalModel.Native global;
			internal fixed byte lfos[Model.LfoCount * LfoModel.Native.Size];
			internal fixed byte envs[Model.EnvCount * EnvModel.Native.Size];
			internal fixed byte units[Model.UnitCount * UnitModel.Native.Size];
			internal fixed byte @params[Model.ParamCount * 8];
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct SyncStep
		{
			internal int num, den;
			internal float val => num / (float)den;

			public override string ToString() => $"{num}/{den}";
			public override int GetHashCode() => num + 37 * den;
			public override bool Equals(object obj) => ((SyncStep)obj).num == num && ((SyncStep)obj).den == den;
			internal SyncStep Simplify() => new SyncStep { num = num / GCD(num, den), den = den / GCD(num, den) };
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

		public SynthModel()
		{
			Envs[0].On.Value = 1;
			Units[0].On.Value = 1;
			Plot.Type.Value = (int)PlotType.Unit1;
			var @params = ListParams(this).Select((p, i) => new SynthParam((IThemedSubModel)p.Sub, i + 1, p.Param));
			SynthParams = new ReadOnlyCollection<SynthParam>(@params.ToArray());
			if (SynthParams.Count != Model.ParamCount)
				throw new InvalidOperationException();
		}

		public void PrepareNative(IntPtr native)
		{
			Native* ptr = (Native*)native;
			var @params = (int**)ptr->@params;
			for (int p = 0; p < Params.Count; p++)
				@params[p] = Params[p].Info.Address(SynthParams[p].Owner.Address(ptr));
		}
	}
}