﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitWave { Sin, Saw, Sqr, Tri }
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class UnitModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int on, amp, oct, note, cent, wave; }

		public Param On { get; } = new(OnInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Wave { get; } = new(WaveInfo);

		readonly int _index;
		public string Name => $"Unit {_index + 1}";
		internal UnitModel(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { On, Wave, Amp, Oct, Note, Cent };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[_index * TrackConstants.UnitModelSize];

		static readonly string[] Waves = Enum.GetValues<UnitWave>().Select(v => v.ToString()).ToArray();
		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		static readonly ParamInfo OnInfo = new ToggleInfo(p => &((Native*)p)->on, nameof(On), "Enabled");
		static readonly ParamInfo AmpInfo = new ContinuousInfo(p => &((Native*)p)->amp, nameof(Amp), "Volume", 255);
		static readonly ParamInfo CentInfo = new DiscreteInfo(p => &((Native*)p)->cent, nameof(Cent), "Cent", -50, 49, 0);
		static readonly ParamInfo NoteInfo = new EnumInfo<UnitNote>(p => &((Native*)p)->note, nameof(Note), "Note", Notes);
		static readonly ParamInfo WaveInfo = new EnumInfo<UnitWave>(p => &((Native*)p)->wave, nameof(Wave), "Waveform", Waves);
		static readonly ParamInfo OctInfo = new DiscreteInfo(p => &((Native*)p)->oct, nameof(Oct), "Octave", TrackConstants.MinOctave, TrackConstants.MaxOctave, 4);
	}
}