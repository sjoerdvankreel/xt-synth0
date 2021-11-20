namespace Xt.Synth0.Model
{
	internal static class ParamInfo
	{
		internal static ParamInfo<bool> Of(string name, bool default_)
		=> new(ParamType.Bool, name, false, true, default_);
	}

	public class ParamInfo<T>
	{
		public T Min { get; }
		public T Max { get; }
		public T Default { get; }
		public string Name { get; }
		public ParamType Type { get; }

		internal ParamInfo(ParamType type, string name, T min, T max, T default_)
		=> (Type, Name, Min, Max, Default) = (type, name, min, max, default_);
	}
}