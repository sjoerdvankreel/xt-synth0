﻿using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotType { Off, Unit1, Unit2, Unit3, Env1, Env2, Env3, LFO1, LFO2, SynthL, SynthR }

	public unsafe sealed class PlotModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int type, hold; }

		public Param Type { get; } = new(TypeInfo);
		public Param Hold { get; } = new(HoldInfo);

		public string Name => "Plot";
		public ThemeGroup Group => ThemeGroup.Plot;
		public IReadOnlyList<Param> Params => new[] { Type, Hold };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;

		static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, nameof(Type), "Source");
		static readonly ParamInfo HoldInfo = ParamInfo.Time(p => &((Native*)p)->hold, nameof(Hold), "Hold key time", 0, 10);
	}
}