using System;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public static class TrackConstants
	{
		public static void SanityChecks()
		{
			VerifySize<SynthModel.Native.Param>(ParamSize);
			VerifySize<AmpModel.Native>(AmpModelSize);
			VerifySize<UnitModel.Native>(UnitModelSize);
			VerifySize<GlobalModel.Native>(GlobalModelSize);
			VerifySize<SynthModel.Native>(SynthModelSize);

			VerifySize<EditModel.Native>(EditModelSize);
			VerifySize<PatternFx.Native>(PatternFxSize);
			VerifySize<PatternKey.Native>(PatternKeySize);
			VerifySize<PatternRow.Native>(PatternRowSize);
			VerifySize<PatternModel.Native>(PatternModelSize);
			VerifySize<SequencerModel.Native>(SequencerModelSize);
		}

		static void VerifySize<T>(int size)
		{
			if (Marshal.SizeOf<T>() != size)
				throw new InvalidOperationException();
		}

		internal const int ParamSize = 16;
		internal const int AmpModelSize = 20;
		internal const int UnitModelSize = 24;
		internal const int GlobalModelSize = 16;
		internal const int SynthModelSize = 544;

		internal const int EditModelSize = 16;
		internal const int PatternFxSize = 8;
		internal const int PatternKeySize = 12;
		internal const int PatternRowSize = 72;
		internal const int PatternModelSize = 18432;
		internal const int SequencerModelSize = 18448;

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