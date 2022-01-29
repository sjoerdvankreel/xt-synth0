using MessagePack;
using System;
using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	[MessagePackObject(keyAsPropertyName: true)]
	public class StoreModel
	{
		public Dictionary<string, int> Params { get; set; } = new();
		public Dictionary<StoreKey, StoreModel> Children { get; set; } = new();
	}

	[MessagePackObject(keyAsPropertyName: true)]
	public class StoreKey: IEquatable<StoreKey>
	{
		public string Id { get;  }
		public int Index { get;  }

		[SerializationConstructor]
		public StoreKey(string id, int index) => (Id, Index) = (id, index);

		public override bool Equals(object obj)=>Equals((StoreKey)obj);
		public override int GetHashCode() => Id.GetHashCode() + 17 * Index;
		public bool Equals(StoreKey other) => Id == other?.Id && Index == other?.Index;
	}
}