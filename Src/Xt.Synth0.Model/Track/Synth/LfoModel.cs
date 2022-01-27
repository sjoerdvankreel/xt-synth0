using MessagePack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum LfoType { Sin, Saw, Sqr, Tri }

	public unsafe sealed class LfoModel : IThemedSubModel, IStoredModel<LfoModel.Native, LfoModel.Native>
	{
		[MessagePackObject(keyAsPropertyName: true)]
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public const int Size = 32;
			public int type, on, sync, inv, bi;
			public int rate, step, pad__;
		};

		public Param On { get; } = new(OnInfo);
		public Param Bi { get; } = new(BiInfo);
		public Param Inv { get; } = new(InvInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Sync { get; } = new(SyncInfo);
		public Param Rate { get; } = new(RateInfo);
		public Param Step { get; } = new(StepInfo);

		readonly int _index;
		public string Name => $"LFO {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Lfo;
		internal LfoModel(int index) => _index = index;
		public void Load(ref Native stored, ref Native native) => native = stored;
		public void Store(ref Native native, ref Native stored) => stored = native;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->lfos[_index * Native.Size];

		public Param Enabled => On;
		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { Sync, 1 } ,{ Rate, 2 }, { Step, 2 },
			{ Inv, 3 }, { Bi, 4 }
		};

		static readonly IRelevance RelevanceSync = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 1);
		static readonly IRelevance RelevanceTime = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 0);

		static readonly ParamInfo InvInfo = ParamInfo.Toggle(p => &((Native*)p)->inv, "Invert", "Invert", false);
		static readonly ParamInfo BiInfo = ParamInfo.Toggle(p => &((Native*)p)->bi, "Bipolar", "Bipolar", false);
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), "Enabled", false);
		static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, nameof(Type), "Type");
		static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, nameof(Sync), "Sync to beat", false);
		static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, nameof(Step), "Rate steps", 1, 7, RelevanceSync);
		static readonly ParamInfo RateInfo = ParamInfo.Time(p => &((Native*)p)->rate, nameof(Rate), "Rate milliseconds", 1, 10, RelevanceTime);
	}
}