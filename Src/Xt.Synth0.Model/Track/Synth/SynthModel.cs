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

		IList<Param> ListParams(IModelGroup group)
		{
			var result = new List<Param>();
			foreach (var model in group.SubModels)
				result.AddRange(model.Params);
			foreach (var child in group.SubGroups)
				result.AddRange(ListParams(child));
			return result;
		}

		public AmpModel Amp { get; } = new();
		public GlobalModel Global { get; } = new();
		public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());

		public IReadOnlyList<Param> Params { get; }
		public event EventHandler<ParamChangedEventArgs> ParamChanged;
		public IReadOnlyList<IModelGroup> SubGroups => new IModelGroup[0];
		public void* Address(void* parent) => throw new NotSupportedException();
		public IReadOnlyList<ISubModel> SubModels => Units.Concat(new ISubModel[] { Amp, Global }).ToArray();
		static IList<UnitModel> MakeUnits() => Enumerable.Range(0, UnitCount).Select(i => new UnitModel(i)).ToList();

		internal SynthModel()
		{
			Units[0].On.Value = 1;
			Params = new ReadOnlyCollection<Param>(ListParams(this));
			for (int p = 0; p < Params.Count; p++)
			{
				int local = p;
				Params[p].PropertyChanged += (s, e) => ParamChanged?.Invoke(this, new ParamChangedEventArgs(local));
			}
		}
	}
}