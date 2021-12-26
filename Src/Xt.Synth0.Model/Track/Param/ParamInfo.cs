using System.Linq;

namespace Xt.Synth0.Model
{
	public abstract class ParamInfo
	{
		public string Name { get; }
		public string Detail { get; }

		public abstract int Min { get; }
		public abstract int Max { get; }
		public abstract int Default { get; }
		public abstract bool IsToggle { get; }

		public abstract string Format(int value);

		int? _maxDisplayLength;
		internal ParamInfo(string name, string detail)
		=> (Name, Detail) = (name, detail);
		public int MaxDisplayLength => _maxDisplayLength ??=
			Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);
	}
}