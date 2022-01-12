using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum SlopeType { Lin, Sqrt, Quad, Log, Exp }

	public unsafe sealed class EnvModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int a, d, s, r, hld, dly;
			internal int aSlope, dSlope, rSlope, pad__;
		}

		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param S { get; } = new(SInfo);
		public Param R { get; } = new(RInfo);
		public Param Hld { get; } = new(HldInfo);
		public Param Dly { get; } = new(DlyInfo);
		public Param ASlope { get; } = new(ASlopeInfo);
		public Param DSlope { get; } = new(DSlopeInfo);
		public Param RSlope { get; } = new(RSlopeInfo);

		readonly int _index;
		public string Name => $"Env {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Envs;
		internal EnvModel(int index) => _index = index;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->envs[_index * TrackConstants.EnvModelSize];

		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ Dly, 0 },
			{ Hld, 1 },
			{ ASlope, 2 },
			{ A, 3 },
			{ DSlope, 4 },
			{ D, 5 },
			{ S, 7 },
			{ RSlope, 8 },
			{ R, 9 }
		};

		static readonly ParamInfo SInfo = ParamInfo.Lin(p => &((Native*)p)->s, nameof(S), 0, 255, 0);
		static readonly ParamInfo AInfo = ParamInfo.Time(p => &((Native*)p)->a, nameof(A), 0, 100, 0);
		static readonly ParamInfo DInfo = ParamInfo.Time(p => &((Native*)p)->d, nameof(D), 0, 100, 0);
		static readonly ParamInfo RInfo = ParamInfo.Time(p => &((Native*)p)->r, nameof(R), 0, 100, 0);
		static readonly ParamInfo HldInfo = ParamInfo.Time(p => &((Native*)p)->hld, nameof(Hld), 0, 100, 0);
		static readonly ParamInfo DlyInfo = ParamInfo.Time(p => &((Native*)p)->dly, nameof(Dly), 0, 100, 0);
		static readonly ParamInfo ASlopeInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->aSlope, nameof(ASlope));
		static readonly ParamInfo DSlopeInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->dSlope, nameof(DSlope));
		static readonly ParamInfo RSlopeInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->rSlope, nameof(RSlope));
	}
}