using MessagePack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum GlobalAmpLfo { LFO1, LFO2 }
	public enum GlobalAmpEnv { Env1, Env2, Env3 }

	public unsafe sealed class GlobalModel : IThemedSubModel, IStoredModel<GlobalModel.Native, GlobalModel.Native>
	{
		[MessagePackObject(keyAsPropertyName: true)]
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public int ampEnv, ampLfo;
			public int amp, ampEnvAmt, ampLfoAmt, pad__;
		}

		public Param Amp { get; } = new(AmpInfo);
		public Param AmpEnv { get; } = new(AmpEnvInfo);
		public Param AmpLfo { get; } = new(AmpLfoInfo);
		public Param AmpEnvAmt { get; } = new(AmpEnvAmtInfo);
		public Param AmpLfoAmt { get; } = new(AmpLfoAmtInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public void Load(in Native stored, out Native native) => native = stored;
		public void Store(in Native native, out Native stored) => stored = native;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ Amp, 0 }, { AmpEnv, 1 }, { AmpEnvAmt, 2 },
			{ AmpLfo, 4 }, { AmpLfoAmt, 5 }
		};

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", 0);
		static readonly ParamInfo AmpLfoAmtInfo = ParamInfo.Level(p => &((Native*)p)->ampLfoAmt, "Amt", "Amp lfo amount", 0);
		static readonly ParamInfo AmpLfoInfo = ParamInfo.List<GlobalAmpLfo>(p => &((Native*)p)->ampLfo, "LFO", "Amp lfo source");
		static readonly ParamInfo AmpEnvAmtInfo = ParamInfo.Level(p => &((Native*)p)->ampEnvAmt, "Amt", "Amp env amount", 255);
		static readonly ParamInfo AmpEnvInfo = ParamInfo.List<GlobalAmpEnv>(p => &((Native*)p)->ampEnv, "Env", "Amp env source");
	}
}