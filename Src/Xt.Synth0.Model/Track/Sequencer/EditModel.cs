﻿using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native { internal int fx, act, pats, keys; }

		public Param Fx { get; } = new(FxInfo);
		public Param Act { get; } = new(ActInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Keys { get; } = new(KeysInfo);

		public string Name => "Edit";
		public IReadOnlyList<Param> Params => new[] { Keys, Fx, Pats, Act };
		public void* Address(void* parent) => &((SequencerModel.Native*)parent)->edit;

		static readonly ParamInfo FxInfo = new DiscreteInfo(p => &((Native*)p)->fx, nameof(Fx), "Effect count", 0, TrackConstants.MaxFxCount, 1);
		static readonly ParamInfo KeysInfo = new DiscreteInfo(p => &((Native*)p)->keys, nameof(Keys), "Note count", 1, TrackConstants.MaxKeyCount, 2);
		static readonly ParamInfo ActInfo = new DiscreteInfo(p => &((Native*)p)->act, nameof(Act), "Active pattern", 1, TrackConstants.PatternCount, 1);
		static readonly ParamInfo PatsInfo = new DiscreteInfo(p => &((Native*)p)->pats, nameof(Pats), "Pattern count", 1, TrackConstants.PatternCount, 1);
	}
}