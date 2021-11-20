using Newtonsoft.Json;
using System;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	internal class ParamConverter<T> : JsonConverter<Param<T>>
	{
		public override void WriteJson(JsonWriter w, Param<T> v, JsonSerializer s)
		=> w.WriteValue(v.Value);

		public override Param<T> ReadJson(JsonReader r, Type t, Param<T> v, bool h, JsonSerializer s)
		{
			v.Value = (T)Convert.ChangeType(r.Value, typeof(T));
			return v;
		}
	}
}