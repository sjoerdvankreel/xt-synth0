using MessagePack;
using System;
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
			public Native(in Stored stored) { }
		}

		[MessagePackObject(keyAsPropertyName: true)]
		public struct Stored
		{
			public PatternFx.Native[] fx = new PatternFx.Native[Model.MaxFxs];
			public PatternKey.Native[] keys = new PatternKey.Native[Model.MaxKeys];
			public Stored(in Native native) { }
		}

		readonly int _index;
		internal PatternRow(int index) => _index = index;
		public IReadOnlyList<IModelContainer> SubContainers => new IModelContainer[0];
		public void Load(in Stored stored, out Native native) => native = new(in stored);
		public void Store(in Native native, out Stored stored) => stored = new(in native);
		public IReadOnlyList<ISubModel> SubModels => Fx.Concat<ISubModel>(Keys).ToArray();
		public void* Address(void* parent) => &((PatternModel.Native*)parent)->rows[_index * Native.Size];

		public IReadOnlyList<PatternFx> Fx = new ReadOnlyCollection<PatternFx>(MakeFx());
		public IReadOnlyList<PatternKey> Keys = new ReadOnlyCollection<PatternKey>(MakeKeys());
		static IList<PatternFx> MakeFx() => Enumerable.Range(0, Model.MaxFxs).Select(i => new PatternFx(i)).ToList();
		static IList<PatternKey> MakeKeys() => Enumerable.Range(0, Model.MaxKeys).Select(i => new PatternKey(i)).ToList();
	}
}