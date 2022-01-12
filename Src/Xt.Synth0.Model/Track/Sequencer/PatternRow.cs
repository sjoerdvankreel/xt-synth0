using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternRow : IModelContainer
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal fixed byte fx[TrackConstants. MaxFxs * TrackConstants.PatternFxSize];
			internal fixed byte keys[TrackConstants.MaxKeys * TrackConstants.PatternKeySize];
		}

		readonly int _index;
		internal PatternRow(int index) => _index = index;
		public IReadOnlyList<IModelContainer> SubContainers => new IModelContainer[0];
		public IReadOnlyList<ISubModel> SubModels => Fx.Concat<ISubModel>(Keys).ToArray();
		public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[_index * TrackConstants.PatternRowSize];

		public IReadOnlyList<PatternFx> Fx = new ReadOnlyCollection<PatternFx>(MakeFx());
		public IReadOnlyList<PatternKey> Keys = new ReadOnlyCollection<PatternKey>(MakeKeys());
		static IList<PatternFx> MakeFx() => Enumerable.Range(0, TrackConstants.MaxFxs).Select(i => new PatternFx(i)).ToList();
		static IList<PatternKey> MakeKeys() => Enumerable.Range(0, TrackConstants.MaxKeys).Select(i => new PatternKey(i)).ToList();
	}
}