namespace Xt.Synth0.Model
{
	public abstract class SubModel : Model
	{
		readonly Param[] _params;
		public Param[] Params() => _params;
		internal abstract Param[] ListParams();
		internal SubModel() => _params = ListParams();

		public override sealed void CopyTo(Model model)
		{
			var sub = (SubModel)model;
			for (int p = 0; p < _params.Length; p++)
				sub._params[p].Value = _params[p].Value;
		}
	}
}