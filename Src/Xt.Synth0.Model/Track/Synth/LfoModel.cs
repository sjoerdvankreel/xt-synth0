using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum LfoType { Sin, Saw, Sqr, Tri }

	public unsafe sealed class LfoModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native
		{
			internal const int Size = 32;
			internal int on, sync, inv, key;
			internal int type, rate, step, pad__;
		};

		public Param On { get; } = new(OnInfo);
		public Param Key { get; } = new(KeyInfo);
		public Param Inv { get; } = new(InvInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Sync { get; } = new(SyncInfo);
		public Param Rate { get; } = new(RateInfo);
		public Param Step { get; } = new(StepInfo);

		readonly int _index;
		public string Name => $"LFO {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Lfo;
		internal LfoModel(int index) => _index = index;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->lfos[_index * Native.Size];

		public Param Enabled => On;
		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { Sync, 1 }, { Rate, 2 }, { Step, 2 },
			{ Key, 4 }, { Inv, 5 }
		};

		static readonly IRelevance RelevanceSync = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 1);
		static readonly IRelevance RelevanceTime = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 0);

		static readonly ParamInfo InvInfo = ParamInfo.Toggle(p => &((Native*)p)->inv, "Invert", "Invert", true, false);
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), "Enabled", false, false);
		static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, nameof(Type), "Type", true);
		static readonly ParamInfo KeyInfo = ParamInfo.Toggle(p => &((Native*)p)->key, nameof(Key), "Sync to key", true, true);
		static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, nameof(Sync), "Sync to beat", true, false);
		static readonly ParamInfo RateInfo = ParamInfo.Time(p => &((Native*)p)->rate, nameof(Rate), "Rate milliseconds", true, 1, 10, RelevanceTime);
		static readonly ParamInfo StepInfo = ParamInfo.Select(p => &((Native*)p)->step, nameof(Step), "Rate steps", true, SyncStep.S1_16, SyncStep.S1_4, SynthModel.SyncStepNames, RelevanceSync);
	}
}