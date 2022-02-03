using System;
using System.Collections.Generic;

namespace Xt.Synth0.UI
{
	public class RequestPlotDataEventArgs : EventArgs
	{
		public float Min { get; set; }
		public float Max { get; set; }
		public bool Clip { get; set; }
		public float Freq { get; set; }
		public int Pixels { get; set; }
		public bool Spectrum { get; set; }
		public float SampleRate { get; set; }
		public List<int> HSplits { get; } = new();
		public List<float> VSplits { get; } = new();
		public List<float> Samples { get; } = new();
	}
}