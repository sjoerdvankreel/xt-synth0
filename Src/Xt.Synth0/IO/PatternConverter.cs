using Newtonsoft.Json;
using System;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class PatternConverter : JsonConverter<PatternModel>
	{
		public override void WriteJson(
			JsonWriter w, PatternModel v, JsonSerializer s)
		{
			w.WriteStartArray();
			foreach (var row in v.Rows)
				s.Serialize(w, row);
			w.WriteEndArray();
		}

		public override PatternModel ReadJson(
			JsonReader r, Type t, PatternModel v, bool h, JsonSerializer s)
		{
			var rows = (PatternRow[])s.Deserialize(r, typeof(PatternRow[]));
			for (int i = 0; i < rows.Length; i++)
				rows[i].CopyTo(v.Rows[i]);
			return v;
		}
	}
}