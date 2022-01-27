using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternRow : IModelContainer
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 88;
			internal fixed byte fx[Model. MaxFxs * PatternFx.Native.Size];
			internal fixed byte keys[Model.MaxKeys * PatternKey.Native.Size];
		}

		readonly int _index;
		internal PatternRow(int index) => _index = index;
		public IReadOnlyList<IModelContainer> SubContainers => new IModelContainer[0];
		public IReadOnlyList<ISubModel> SubModels => Fx.Concat<ISubModel>(Keys).ToArray();
		public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[_index * Native.Size];

		public IReadOnlyList<PatternFx> Fx = new ReadOnlyCollection<PatternFx>(MakeFx());
		public IReadOnlyList<PatternKey> Keys = new ReadOnlyCollection<PatternKey>(MakeKeys());
		static IList<PatternFx> MakeFx() => Enumerable.Range(0, Model.MaxFxs).Select(i => new PatternFx(i)).ToList();
		static IList<PatternKey> MakeKeys() => Enumerable.Range(0, Model.MaxKeys).Select(i => new PatternKey(i)).ToList();
	}
}