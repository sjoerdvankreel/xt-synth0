using Newtonsoft.Json;
using System;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	internal class ParamConverter : JsonConverter<Param>
	{
		public override void WriteJson(JsonWriter w, Param v, JsonSerializer s)
		=> w.WriteValue(v.Value);

		public override Param ReadJson(JsonReader r, Type t, Param v, bool h, JsonSerializer s)
		{
			v.Value = (int)Convert.ChangeType(r.Value, typeof(int));
			return v;
		}
	}
}