using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class SubModel : ICopyModel
	{
		readonly Param[] _params;
		internal Param[] Params() => _params;
		internal abstract IEnumerable<Param> ListParams();
		internal SubModel() => _params = ListParams().ToArray();

		public void CopyTo(ICopyModel model)
		{
			var sub = (SubModel)model;
			for (int p = 0; p < _params.Length; p++)
				sub._params[p].Value = _params[p].Value;
		}
	}
}