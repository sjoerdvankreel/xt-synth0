using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternRow : IGroupContainerModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 88;
			internal fixed byte fx[Model. MaxFxs * PatternFx.Native.Size];
			internal fixed byte keys[Model.MaxKeys * PatternKey.Native.Size];
		}

		public int Index { get; }
		public string Id => "5E1E96A6-1210-4FC4-BB3F-042C854935A5";
		public IReadOnlyList<IGroupContainerModel> Children => new IGroupContainerModel[0];
		public IReadOnlyList<IParamGroupModel> Groups => Fx.Concat<IParamGroupModel>(Keys).ToArray();
		public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[Index * Native.Size];

		internal PatternRow(int index) => Index = index;
		public IReadOnlyList<PatternFx> Fx = new ReadOnlyCollection<PatternFx>(MakeFx());
		public IReadOnlyList<PatternKey> Keys = new ReadOnlyCollection<PatternKey>(MakeKeys());

		static IList<PatternFx> MakeFx() => Enumerable.Range(0, Model.MaxFxs).Select(i => new PatternFx(i)).ToList();
		static IList<PatternKey> MakeKeys() => Enumerable.Range(0, Model.MaxKeys).Select(i => new PatternKey(i)).ToList();
	}
}