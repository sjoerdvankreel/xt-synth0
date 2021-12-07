using Newtonsoft.Json;
using System;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class ModelListConverter<T> : JsonConverter<ModelList<T>>
		where T : ICopyModel
	{
		public override void WriteJson(
			JsonWriter w, ModelList<T> v, JsonSerializer s)
		{
			w.WriteStartArray();
			foreach (var e in v.Items)
				s.Serialize(w, e);
			w.WriteEndArray();
		}

		public override ModelList<T> ReadJson(
			JsonReader r, Type t, ModelList<T> v, bool h, JsonSerializer s)
		{
			var items = (T[])s.Deserialize(r, typeof(T[]));
			if (v.Items.Count != items.Length)
				throw new InvalidOperationException();
			for (int i = 0; i < items.Length; i++)
				items[i].CopyTo(v.Items[i]);
			return v;
		}
	}
}