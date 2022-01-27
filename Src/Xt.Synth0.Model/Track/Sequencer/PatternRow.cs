using MessagePack;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternRow : IModelContainer, IStoredModel<PatternRow.Native, PatternRow.Stored>
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public const int Size = 88;
			public fixed byte fx[Model.MaxFxs * PatternFx.Native.Size];
			public fixed byte keys[Model.MaxKeys * PatternKey.Native.Size];
		}

		[MessagePackObject(keyAsPropertyName: true)]
		public struct Stored
		{
			public PatternFx.Native[] fx = new PatternFx.Native[Model.MaxFxs];
			public PatternKey.Native[] keys = new PatternKey.Native[Model.MaxKeys];
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

		public void Store(ref Native native, ref Stored stored)
		{
			fixed (byte* fx = native.fx)
				for (int i = 0; i < Model.MaxFxs; i++)
					stored.fx[i] = ((PatternFx.Native*)fx)[i];
			fixed (byte* keys = native.keys)
				for (int i = 0; i < Model.MaxKeys; i++)
					stored.keys[i] = ((PatternKey.Native*)keys)[i];
		}

		public void Load(ref Stored stored, ref Native native)
		{
			fixed (byte* fx = native.fx)
				for (int i = 0; i < Model.MaxFxs && i < stored.fx.Length; i++)
					((PatternFx.Native*)fx)[i] = stored.fx[i];
			fixed (byte* keys = native.keys)
				for (int i = 0; i < Model.MaxKeys && i < stored.keys.Length; i++)
					((PatternKey.Native*)keys)[i] = stored.keys[i];
		}
	}
}