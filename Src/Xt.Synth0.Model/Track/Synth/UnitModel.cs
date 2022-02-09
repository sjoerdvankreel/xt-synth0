using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitType { Add, Blep }
	public enum BlepType { Saw, Pulse, Tri }
	public enum ModSource { Off, Env1, Env2, Env3, LFO1, LFO2 }
	public enum ModTarget { Off, Pw, Amp, Pan, Roll, Freq, Phase };
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class UnitModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 80;
			internal int on, type, note, addSub, blepType;
			internal int amt1, amt2, src1, src2, tgt1, tgt2;
			internal int amp, pan, oct, dtn, pw;
			internal int addParts, addStep, addRoll, pad__;
		}

		public Param On { get; } = new(OnInfo);
		public Param Pw { get; } = new(PwInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Pan { get; } = new(PanInfo);
		public Param Dtn { get; } = new(DtnInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Src1 { get; } = new(Src1Info);
		public Param Tgt1 { get; } = new(Tgt1Info);
		public Param Amt1 { get; } = new(Amt1Info);
		public Param Src2 { get; } = new(Src2Info);
		public Param Tgt2 { get; } = new(Tgt2Info);
		public Param Amt2 { get; } = new(Amt2Info);
		public Param AddSub { get; } = new(AddSubInfo);
		public Param AddStep { get; } = new(AddStepInfo);
		public Param AddRoll { get; } = new(AddRollInfo);
		public Param AddParts { get; } = new(AddPartsInfo);
		public Param BlepType { get; } = new(BlepTypeInfo);

		public int Columns => 3;
		public int Index { get; }
		public Param Enabled => On;
		public bool AutoSizeCols => true;
		public string Name => $"Unit {Index + 1}";
		public ThemeGroup ThemeGroup => ThemeGroup.Unit;
		public string Id => "3DACD0A4-9688-4FA9-9CA3-B8A0E49A45E5";
		public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[Index * Native.Size];
		public IDictionary<Param, int> Layout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { AddSub, 1 }, { BlepType, 1 },
			{ Amp, 3 }, { Pan, 4 }, { Pw, 5 },
			{ Oct, 6 }, { Note, 7 }, { Dtn, 8 },
			{ AddParts, 9 }, { AddStep, 10 }, { AddRoll, 11 },
			{ Src1, 12 }, { Tgt1, 13 }, {Amt1, 14 },
			{ Src2, 15 }, { Tgt2, 16}, {Amt2, 17 },
		};

		internal UnitModel(int index) => Index = index;
		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		static readonly IRelevance RelevanceAdd = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add);
		static readonly IRelevance RelevanceBlep = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Blep);
		static readonly IRelevance RelevancePw = Relevance.All(
			Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Blep),
			Relevance.When((UnitModel m) => m.BlepType, (BlepType t) => t != Synth0.Model.BlepType.Saw));

		static readonly ParamInfo DtnInfo = ParamInfo.Mix(p => &((Native*)p)->dtn, nameof(Dtn), nameof(Dtn), "Detune");
		static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, nameof(Pan), nameof(Pan), "Panning");
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), nameof(On), "Enabled", false);
		static readonly ParamInfo Amt1Info = ParamInfo.Level(p => &((Native*)p)->amt1, nameof(Amt1), "Amt", "Mod 1 amount", 0);
		static readonly ParamInfo Amt2Info = ParamInfo.Level(p => &((Native*)p)->amt2, nameof(Amt2), "Amt", "Mod 2 amount", 0);
		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), nameof(Amp), "Amplitude", 255);
		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, nameof(Oct), nameof(Oct), "Octave", 0, 9, 4);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type), nameof(Type), "Type");
		static readonly ParamInfo PwInfo = ParamInfo.Level(p => &((Native*)p)->pw, nameof(Pw), "PW", "Pulse width", 0, RelevancePw);
		static readonly ParamInfo Src1Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src1, nameof(Src1), "Source", "Mod 1 source");
		static readonly ParamInfo Tgt1Info = ParamInfo.List<ModTarget>(p => &((Native*)p)->tgt1, nameof(Tgt1), "Target", "Mod 1 target");
		static readonly ParamInfo Src2Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src2, nameof(Src2), "Source", "Mod 2 source");
		static readonly ParamInfo Tgt2Info = ParamInfo.List<ModTarget>(p => &((Native*)p)->tgt2, nameof(Tgt2), "Target", "Mod 2 target");
		static readonly ParamInfo NoteInfo = ParamInfo.Select<UnitNote>(p => &((Native*)p)->note, nameof(Note), nameof(Note), "Note", Notes);
		static readonly ParamInfo AddRollInfo = ParamInfo.Mix(p => &((Native*)p)->addRoll, nameof(AddRoll), "Roll", "Additive rolloff", RelevanceAdd);
		static readonly ParamInfo AddSubInfo = ParamInfo.Toggle(p => &((Native*)p)->addSub, nameof(AddSub), "Sub", "Additive subtract", false, RelevanceAdd);
		static readonly ParamInfo AddStepInfo = ParamInfo.Select(p => &((Native*)p)->addStep, nameof(AddStep), "Step", "Additive step", 1, 32, 1, RelevanceAdd);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Select(p => &((Native*)p)->addParts, nameof(AddParts), "Parts", "Additive partials", 1, 32, 1, RelevanceAdd);
		static readonly ParamInfo BlepTypeInfo = ParamInfo.List<BlepType>(p => &((Native*)p)->blepType, nameof(BlepType), "Type", "Blep type", null, RelevanceBlep);
	}
}