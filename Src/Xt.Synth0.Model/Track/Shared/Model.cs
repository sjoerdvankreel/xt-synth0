﻿namespace Xt.Synth0.Model
{
	public static class Model
	{
		public static int MainThreadId { get; set; } = -1;

		
		public const int ParamCount = 209;

		public const int MaxFxs = 3;
		public const int MaxLpb = 16;
		public const int MaxKeys = 4;
		public const int MaxRows = 32;
		public const int MaxPatterns = 8;
		public const int TotalRows = MaxPatterns * MaxRows;
	}
}