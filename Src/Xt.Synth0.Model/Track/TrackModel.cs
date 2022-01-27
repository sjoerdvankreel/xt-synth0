using MessagePack;
using System;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class TrackModel : IStoredModel<TrackModel.Native, TrackModel.Stored>
	{
		[MessagePackObject(keyAsPropertyName: true)]
		public struct Stored
		{
			public SeqModel.Stored seq;
			public SynthModel.Stored synth;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public SeqModel.Native seq;
			public SynthModel.Native synth;
		}

		public SeqModel Seq { get; } = new();
		public SynthModel Synth { get; } = new();

		public event EventHandler ParamChanged;
		public unsafe void* Address(void* parent) => throw new NotSupportedException();

		public TrackModel()
		{
			Synth.ParamChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
			Seq.ParamChanged += (s, e) => ParamChanged?.Invoke(this, EventArgs.Empty);
		}

		public void CopyTo(TrackModel track)
		{
			Synth.CopyTo(track.Synth);
			Seq.CopyTo(track.Seq);
		}

		public void Load(ref Stored stored, ref Native native)
		{
			Seq.Load(ref stored.seq, ref native.seq);
			Synth.Load(ref stored.synth, ref native.synth);
		}

		public void Store(ref Native native, ref Stored stored)
		{
			Seq.Store(ref native.seq, ref stored.seq);
			Synth.Store(ref native.synth, ref stored.synth);
		}
	}
}