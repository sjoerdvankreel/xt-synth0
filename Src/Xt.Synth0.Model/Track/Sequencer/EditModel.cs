﻿using MessagePack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class EditModel : IThemedSubModel, IStoredModel<EditModel.Native, EditModel.Native>
	{
		[MessagePackObject(keyAsPropertyName: true)]
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native { public int pats, rows, keys, fxs, lpb, edit, bpm, pad__; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Fxs { get; } = new(FxsInfo);
		public Param Lpb { get; } = new(LpbInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Rows { get; } = new(RowsInfo);

		public int ColumnCount => 2;
		public string Name => "Edit";
		public ThemeGroup Group => ThemeGroup.Pattern;
		public void* Address(void* parent) => &((SeqModel.Native*)parent)->edit;
		public void Load(in Native stored, out Native native) => native = stored;
		public void Store(in Native native, out Native stored) => stored = native;
		public IReadOnlyList<Param> Params => new[] { Pats, Rows, Keys, Fxs, Bpm, Lpb, Edit };

		static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, "BPM", "Beats per minute", 1, 255, 120);
		static readonly ParamInfo LpbInfo = ParamInfo.Select(p => &((Native*)p)->lpb, "LPB", "Lines per beat", 1, Model.MaxLpb, 4);
		static readonly ParamInfo FxsInfo = ParamInfo.Select(p => &((Native*)p)->fxs, nameof(Fxs), "Effect count", 0, Model.MaxFxs, 1);
		static readonly ParamInfo KeysInfo = ParamInfo.Select(p => &((Native*)p)->keys, nameof(Keys), "Key count", 1, Model.MaxKeys, 2);
		static readonly ParamInfo PatsInfo = ParamInfo.Select(p => &((Native*)p)->pats, nameof(Pats), "Pattern count", 1, Model.MaxPatterns, 1);
		static readonly ParamInfo EditInfo = ParamInfo.Select(p => &((Native*)p)->edit, nameof(Edit), "Active pattern", 1, Model.MaxPatterns, 1);
		static readonly ParamInfo RowsInfo = ParamInfo.Select(p => &((Native*)p)->rows, nameof(Rows), "Rows per pattern", 1, Model.MaxRows, Model.MaxRows);
	}
}