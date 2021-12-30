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
		static SynthModel()
		{
			if (Size != XtsSynthModelSize())
				throw new InvalidOperationException();
		}

		internal const int Size = 1;
		public const int UnitCount = 3;
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsSynthModelSize();
		[StructLayout(LayoutKind.Sequential)]
		internal unsafe struct Native
		{
			internal AmpModel.Native amp;
			internal GlobalModel.Native global;
			internal fixed byte units[UnitCount * UnitModel.Size];
		}

		public AmpModel Amp { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		public SynthModel() => Units[0].On.Value = 1;
		public IReadOnlyList<IModelGroup> SubGroups => new IModelGroup[0];
		public void* Address(void* parent) => throw new NotSupportedException();
		public IReadOnlyList<ISubModel> SubModels => Units.Concat(new ISubModel[] { Amp, Global }).ToArray();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, UnitCount).Select(i => new UnitModel(i)).ToList();
	}
}