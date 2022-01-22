using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class ControlModel : IThemedModel
	{
		public string Name => "Control";
		public ThemeGroup Group => ThemeGroup.MonitorControl;
	}

	public sealed class MonitorModel : IThemedModel
	{
		public string Name => "Monitor";
		public ThemeGroup Group => ThemeGroup.MonitorControl;
	}

	public unsafe sealed class SeqModel : MainModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		struct Param
		{
			internal const int Size = 16;
			internal int* val; internal int min, max;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native
		{
			internal EditModel.Native edit;
			internal PatternModel.Native pattern;
			internal fixed byte @params[Model.ParamCount * Param.Size];
		}

		public EditModel Edit { get; } = new();
		public PatternModel Pattern { get; } = new();
		public ControlModel Control { get; } = new();
		public MonitorModel Monitor { get; } = new();
		public override IReadOnlyList<ISubModel> SubModels => new[] { Edit };
		public override IReadOnlyList<IModelContainer> SubContainers => new[] { Pattern };

		public static void PrepareNative(SynthModel synth, IntPtr nativeSeq, IntPtr nativeSynth)
		{
			Native* seqPtr = (Native*)nativeSeq;
			var @params = (Param*)seqPtr->@params;
			SynthModel.Native* synthPtr = (SynthModel.Native*)nativeSynth;
			for (int p = 0; p < synth.AutoParams.Count; p++)
			{
				@params[p].min = synth.AutoParams[p].Param.Info.Min;
				@params[p].max = synth.AutoParams[p].Param.Info.Max;
				var addr = synth.AutoParams[p].Owner.Address(synthPtr);
				@params[p].val = synth.AutoParams[p].Param.Info.Address(addr);
			}
		}
	}
}