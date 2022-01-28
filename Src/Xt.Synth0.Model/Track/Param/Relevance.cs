using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public interface IRelevance
	{
		Param[] Params(IParamGroupModel model);
		bool Relevant(IParamGroupModel model, int[] values);
	}

	class CombinedRelevance : IRelevance
	{
		readonly bool _all;
		readonly IRelevance[] _relevance;
		public Param[] Params(IParamGroupModel model) => _relevance.SelectMany(r => r.Params(model)).ToArray();
		internal CombinedRelevance(IRelevance[] relevance, bool all) => (_relevance, _all) = (relevance, all);

		public bool Relevant(IParamGroupModel model, int[] values)
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
			return _all;
		}
	}

	class Relevance : IRelevance
	{
		readonly Func<int, bool> _relevant;
		readonly Func<IParamGroupModel, Param> _param;

		public Param[] Params(IParamGroupModel model) => new[] { _param(model) };
		public bool Relevant(IParamGroupModel model, int[] values) => _relevant(values.Single());
		internal Relevance(Func<IParamGroupModel, Param> param, Func<int, bool> relevant) => (_param, _relevant) = (param, relevant);

		internal static IRelevance All(params IRelevance[] relevance) => new CombinedRelevance(relevance, true);
		internal static IRelevance Any(params IRelevance[] relevance) => new CombinedRelevance(relevance, false);
		internal static IRelevance When<TModel, TValue>(Func<TModel, Param> param, Func<TValue, bool> relevant)
		where TModel : IParamGroupModel => new Relevance(m => param((TModel)m), v => relevant((TValue)(object)v));
	}
}