namespace Xt.Synth0.Model
{
	public class ParamInfo
	{
		public int Min { get; }
		public int Max { get; }
		public int Default { get; }
		public string Name { get; }
		public ParamType Type { get; }

		internal ParamInfo(string name)
		=> (Type, Name, Min, Max, Default) = (ParamType.Toggle, name, 0, 1, 0);
		internal ParamInfo(ParamType type, string name, int min, int max, int @default)
		=> (Type, Name, Min, Max, Default) = (type, name, min, max, @default);
	}
}