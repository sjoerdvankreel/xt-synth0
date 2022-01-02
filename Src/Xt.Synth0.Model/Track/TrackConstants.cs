namespace Xt.Synth0.Model
{
	public static class TrackConstants
	{
		internal const int ParamSize = 16;
		internal const int UnitSize = 24;
		internal const int PatternFxSize = 8;
		internal const int PatternKeySize = 12;
		internal const int PatternRowSize = 72;

		public const int MinOctave = 0;
		public const int MaxOctave = 9;
		public const int FormatVersion = 1;

		public const int UnitCount = 3;
		public const int ParamCount = 27;
		public const int MaxFxCount = 3;
		public const int MaxKeyCount = 4;
		public const int PatternRows = 32;
		public const int PatternCount = 8;
		public const int TotalRowCount = PatternCount * PatternRows;
	}
}