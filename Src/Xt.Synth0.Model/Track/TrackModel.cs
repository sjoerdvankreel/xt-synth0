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
			public Stored(in Native native): this() { }
		}

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public SeqModel.Native seq;
			public SynthModel.Native synth;
			public Native(in Stored stored) : this() { }
		}

		public SeqModel Seq { get; } = new();
		public SynthModel Synth { get; } = new();

		public event EventHandler ParamChanged;
		public unsafe void* Address(void* parent) => throw new NotSupportedException();
		public void Load(in Stored stored, out Native native) => native = new(in stored);
		public void Store(in Native native, out Stored stored) => stored = new(in native);

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
	}
}