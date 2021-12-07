﻿using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class GroupModel : SubModel
	{
		readonly string _name;
		public string Name() => _name;

		public virtual bool Automation() => true;
		internal abstract Param[][] ListParamGroups();

		readonly Param[][] _paramGroups;
		public Param[][] ParamGroups() => _paramGroups;

		internal GroupModel(string name)
		=> (_name, _paramGroups) = (name, ListParamGroups());
		internal override sealed IEnumerable<Param> ListParams()
		=> ListParamGroups().SelectMany(g => g);
	}
}