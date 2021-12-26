using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class MainModel : ICopyModel
	{
		readonly SubModel[] _subModels;
		internal SubModel[] SubModels() => _subModels;
		internal abstract IEnumerable<SubModel> ListSubModels();
		internal MainModel() => _subModels = ListSubModels().ToArray();

		public void CopyTo(ICopyModel model)
		{
			var main = (MainModel)model;
			Debug.Assert(ListSubModels().SequenceEqual(_subModels));
			Debug.Assert(_subModels.Length == main._subModels.Length);
			for (int s = 0; s < _subModels.Length; s++)
				_subModels[s].CopyTo(main._subModels[s]);
		}
	}
}