using MessagePack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class SynthModel : MainModel<SynthModel.Native, SynthModel.Stored>
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
		public struct Native
		{
			public PlotModel.Native plot;
			public GlobalModel.Native global;
			public fixed byte lfos[Model.LfoCount * LfoModel.Native.Size];
			public fixed byte envs[Model.EnvCount * EnvModel.Native.Size];
			public fixed byte units[Model.UnitCount * UnitModel.Native.Size];
		}

		[MessagePackObject(keyAsPropertyName: true)]
		public struct Stored
		{
			public PlotModel.Native plot;
			public GlobalModel.Native global;
			public LfoModel.Native[] lfos = new LfoModel.Native[Model.LfoCount];
			public EnvModel.Native[] envs = new EnvModel.Native[Model.EnvCount];
			public UnitModel.Native[] units = new UnitModel.Native[Model.UnitCount];
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

		public override void Store(ref Native native, ref Stored stored)
		{
			stored.plot = native.plot;
			stored.global = native.global;
			fixed (byte* lfos = native.lfos)
				for (int i = 0; i < Model.LfoCount; i++)
					Lfos[i].Store(ref ((LfoModel.Native*)lfos)[i], ref stored.lfos[i]);
			fixed (byte* envs = native.envs)
				for (int i = 0; i < Model.EnvCount; i++)
					Envs[i].Store(ref ((EnvModel.Native*)envs)[i], ref stored.envs[i]);
			fixed (byte* units = native.units)
				for (int i = 0; i < Model.UnitCount; i++)
					Units[i].Store(ref ((UnitModel.Native*)units)[i], ref stored.units[i]);
		}

		public override void Load(ref Stored stored, ref Native native)
		{
			native.plot = stored.plot;
			native.global = stored.global;
			fixed (byte* lfos = native.lfos)
				for (int i = 0; i < Model.LfoCount && i < stored.lfos.Length; i++)
					Lfos[i].Load(ref stored.lfos[i], ref ((LfoModel.Native*)lfos)[i]);
			fixed (byte* envs = native.envs)
				for (int i = 0; i < Model.EnvCount && i < stored.envs.Length; i++)
					Envs[i].Load(ref stored.envs[i], ref ((EnvModel.Native*)envs)[i]);
			fixed (byte* units = native.units)
				for (int i = 0; i < Model.UnitCount && i < stored.units.Length; i++)
					Units[i].Load(ref stored.units[i], ref ((UnitModel.Native*)units)[i]);
		}
	}
}