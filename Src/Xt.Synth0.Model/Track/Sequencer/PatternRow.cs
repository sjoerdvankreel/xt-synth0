using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternRow : IModelGroup
	{
		public const int MaxFxCount = 3;
		public const int MaxKeyCount = 4;

		static PatternRow()
		{
			if (Size != XtsPatternRowSize())
				throw new InvalidOperationException();
			if (MaxFxCount != XtsPatternRowMaxFxCount())
				throw new InvalidOperationException();
			if (MaxKeyCount != XtsPatternRowMaxKeyCount())
				throw new InvalidOperationException();
		}

		internal const int Size = 1;
		
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsPatternRowSize();
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsPatternRowMaxFxCount();
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsPatternRowMaxKeyCount();

		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			internal fixed byte fx[MaxFxCount * PatternFx.Size];
			internal fixed byte keys[MaxKeyCount * PatternKey.Size];
		}

		readonly int _index;
		internal PatternRow(int index) => _index = index;
		public IReadOnlyList<IModelGroup> SubGroups => new IModelGroup[0];
		public IReadOnlyList<ISubModel> SubModels => Fx.Concat<ISubModel>(Keys).ToArray();
		public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[_index * Size];

		public IReadOnlyList<PatternFx> Fx = new ReadOnlyCollection<PatternFx>(MakeFx());
		public IReadOnlyList<PatternKey> Keys = new ReadOnlyCollection<PatternKey>(MakeKeys());
		static IList<PatternFx> MakeFx() => Enumerable.Repeat(0, MaxFxCount).Select(i => new PatternFx(i)).ToList();
		static IList<PatternKey> MakeKeys() => Enumerable.Repeat(0, MaxKeyCount).Select(i => new PatternKey(i)).ToList();
	}
}