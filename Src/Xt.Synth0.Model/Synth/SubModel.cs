using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class SubModel : ICopyModel
	{
		readonly Param[] _params;
		public Param[] Params() => _params;

		internal abstract IEnumerable<Param> ListParams();
		internal SubModel() => _params = ListParams().ToArray();

		public void CopyTo(ICopyModel model)
		{
			var sub = (SubModel)model;
			Debug.Assert(ListParams().SequenceEqual(_params));
			if (_params.Length != sub._params.Length)
				throw new InvalidOperationException();
			for (int p = 0; p < _params.Length; p++)
				if (!ReferenceEquals(_params[p].Info, sub._params[p].Info))
					throw new InvalidOperationException();
				else
					sub._params[p].Value = _params[p].Value;
		}
	}
}