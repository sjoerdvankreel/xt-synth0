using System;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public static class TrackConstants
	{
		public static void SanityChecks()
		{
			VerifySize<SeqModel.Native>(SeqModelSize);
			VerifySize<EditModel.Native>(EditModelSize);
			VerifySize<PatternFx.Native>(PatternFxSize);
			VerifySize<PatternKey.Native>(PatternKeySize);
			VerifySize<PatternRow.Native>(PatternRowSize);
			VerifySize<PatternModel.Native>(PatternModelSize);

			VerifySize<UnitModel.Native>(UnitModelSize);
			VerifySize<SynthModel.Native>(SynthModelSize);
			VerifySize<GlobalModel.Native>(GlobalModelSize);
			VerifySize<SynthModel.Native.AutoParam>(AutoParamSize);
		}

		static void VerifySize<T>(int size)
		{
			if (Marshal.SizeOf<T>() != size)
				throw new InvalidOperationException();
		}

		public const int Alignment = 8;
		public const int FormatVersion = 1;

		public const int EnvCount = 2;
		public const int UnitCount = 3;
		public const int AutoParamCount = 59;

		internal const int EnvModelSize = 40;
		internal const int PlotModelSize = 8;
		internal const int UnitModelSize = 56;
		internal const int AutoParamSize = 16;
		internal const int GlobalModelSize = 16;
		internal const int SynthModelSize = 1216;

		internal const int PatternFxSize = 8;
		internal const int EditModelSize = 24;
		internal const int PatternKeySize = 16;
		internal const int PatternRowSize = 88;
		internal const int SeqModelSize = 22552;
		internal const int PatternModelSize = 22528;

		public const int MaxFxs = 3;
		public const int MaxLpb = 16;
		public const int MaxKeys = 4;
		public const int MaxRows = 32;
		public const int MaxPatterns = 8;
		public const int TotalRows = MaxPatterns * MaxRows;
	}
}