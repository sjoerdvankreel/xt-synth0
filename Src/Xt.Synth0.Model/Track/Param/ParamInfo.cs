using System.Linq;

namespace Xt.Synth0.Model
{
	unsafe delegate int* Address(void* parent);

	public abstract class ParamInfo
	{
		Address _address;
		int? _maxDisplayLength;

		public string Name { get; }
		public string Detail { get; }

		public abstract int Min { get; }
		public abstract int Max { get; }
		public abstract int Default { get; }
		public abstract bool IsToggle { get; }

		public abstract string Format(int value);
		public unsafe int* Address(void* native) => _address(native);

		internal ParamInfo(Address address, string name, string detail)
		=> (_address, Name, Detail) = (address, name, detail);
		public int MaxDisplayLength => _maxDisplayLength ??= GetMaxDisplayLength();
		int GetMaxDisplayLength() => Enumerable.Range(Min, Max - Min + 1).Select(Format).Max(t => t.Length);
	}
}