using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	class Relevance
	{
		internal int[] Values { get; }
		internal Func<ISubModel, Param> Param { get; }
		internal Relevance(Func<ISubModel, Param> param, int[] values)
		=> (Param, Values) = (param, values);

		internal static Relevance When<TModel, TValue>(
			Func<TModel, Param> param, params TValue[] values) where TModel : ISubModel
		=> new Relevance(m => param((TModel)m), values.Select(v => Convert.ToInt32(v)).ToArray());
	}
}