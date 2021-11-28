﻿using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class UnitModel : GroupModel
	{
		public static readonly string[] Notes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		public static readonly string[] Types = Enum.
			GetValues<UnitType>().Select(v => v.ToString()).ToArray();

		const string NoteDetail = "Note";
		const string CentDetail = "Cent";
		const string OctDetail = "Octave";
		const string AmpDetail = "Volume";
		const string OnDetail = "Enabled";
		const string TypeDetail = "Waveform";

		static readonly ParamInfo OnInfo = new ToggleInfo(
			nameof(On), OnDetail);
		static readonly ParamInfo OctInfo = new DiscreteInfo(
			nameof(Oct), OctDetail, 0, 12, 4);
		static readonly ParamInfo CentInfo = new DiscreteInfo(
			nameof(Cent), CentDetail, -50, 49, 0);
		static readonly ParamInfo AmpInfo = new ContinuousInfo(
			nameof(Amp), AmpDetail, 255);
		static readonly ParamInfo NoteInfo = new EnumInfo<UnitNote>(
			nameof(Note), NoteDetail, Notes);
		static readonly ParamInfo TypeInfo = new EnumInfo<UnitType>(
			nameof(Type), TypeDetail, Types);

		public Param On { get; } = new(OnInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Type { get; } = new(TypeInfo);

		internal UnitModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() => new[] {
			new[] { On, Type },
			new[] { Amp, Oct },
			new[] { Note, Cent }
		};
	}
}