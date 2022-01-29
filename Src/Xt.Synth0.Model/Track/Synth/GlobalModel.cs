using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum GlobalAmpLfo { LFO1, LFO2 }
	public enum GlobalAmpEnv { Env1, Env2, Env3 }

	public unsafe sealed class GlobalModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal int ampEnv, ampLfo;
			internal int amp, ampEnvAmt, ampLfoAmt, pad__;
		}

		public Param Amp { get; } = new(AmpInfo);
		public Param AmpEnv { get; } = new(AmpEnvInfo);
		public Param AmpLfo { get; } = new(AmpLfoInfo);
		public Param AmpEnvAmt { get; } = new(AmpEnvAmtInfo);
		public Param AmpLfoAmt { get; } = new(AmpLfoAmtInfo);

		public int Index => 0;
		public int Columns => 3;
		public Param Enabled => null;
		public string Name => "Global";
		public ThemeGroup ThemeGroup => ThemeGroup.Global;
		public string Id => "F7791FBA-3693-4D71-8EC9-AB507A03FE9A";
		public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;
		public IDictionary<Param, int> Layout => new Dictionary<Param, int>
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