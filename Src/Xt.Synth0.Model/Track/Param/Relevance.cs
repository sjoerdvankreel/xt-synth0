using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public interface IRelevance
	{
		Param[] Params(ISubModel model);
		bool Relevant(ISubModel model, int[] values);
	}

	class CombinedRelevance : IRelevance
	{
		readonly bool _all;
		readonly IRelevance[] _relevance;
		public Param[] Params(ISubModel model) => _relevance.SelectMany(r => r.Params(model)).ToArray();
		internal CombinedRelevance(IRelevance[] relevance, bool all) => (_relevance, _all) = (relevance, all);

		public bool Relevant(ISubModel model, int[] values)
		{
			int p = 0;
			for (int i = 0; i < _relevance.Length; i++)
			{
				var count = _relevance[i].Params(model).Length;
				var subValues = values.Skip(p).Take(count).ToArray();
				var relevant = _relevance[i].Relevant(model, subValues);
				if (relevant && !_all) return true;
				if (!relevant && _all) return false;
				p += subValues.Length;
			}
			return true;
		}
	}

	class Relevance : IRelevance
	{
		readonly int[] _values;
		readonly Func<ISubModel, Param> _param;

		public Param[] Params(ISubModel model) => new[] { _param(model) };
		public bool Relevant(ISubModel model, int[] values) => _values.Contains(values.Single());
		internal Relevance(Func<ISubModel, Param> param, int[] values) => (_param, _values) = (param, values);

		internal static IRelevance All(params IRelevance[] relevance) => new CombinedRelevance(relevance, true);
		internal static IRelevance Any(params IRelevance[] relevance) => new CombinedRelevance(relevance, false);
		internal static IRelevance When<TModel, TValue>(Func<TModel, Param> param, params TValue[] values)
		where TModel : ISubModel => new Relevance(m => param((TModel)m), values.Select(v => Convert.ToInt32(v)).ToArray());
	}
}