using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternRow : IModelGroup
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			internal fixed byte fx[TrackConstants. MaxFxCount * TrackConstants.PatternFxSize];
			internal fixed byte keys[TrackConstants.MaxKeyCount * TrackConstants.PatternKeySize];
		}

		readonly int _index;
		internal PatternRow(int index) => _index = index;
		public IReadOnlyList<IModelGroup> SubGroups => new IModelGroup[0];
		public IReadOnlyList<ISubModel> SubModels => Fx.Concat<ISubModel>(Keys).ToArray();
		public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[_index * TrackConstants.PatternRowSize];

		public IReadOnlyList<PatternFx> Fx = new ReadOnlyCollection<PatternFx>(MakeFx());
		public IReadOnlyList<PatternKey> Keys = new ReadOnlyCollection<PatternKey>(MakeKeys());
		static IList<PatternFx> MakeFx() => Enumerable.Repeat(0, TrackConstants.MaxFxCount).Select(i => new PatternFx(i)).ToList();
		static IList<PatternKey> MakeKeys() => Enumerable.Repeat(0, TrackConstants.MaxKeyCount).Select(i => new PatternKey(i)).ToList();
	}
}