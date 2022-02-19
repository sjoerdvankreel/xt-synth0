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
        public bool Stereo { get; set; }
        public bool Spectrum { get; set; }
		public float SampleRate { get; set; }
		public List<float> LSamples { get; } = new();
        public List<float> RSamples { get; } = new();
        public List<int> HSplitVals { get; } = new();
		public List<float> VSplitVals { get; } = new();
		public List<string> HSplitMarkers { get; set; } = new();
		public List<string> VSplitMarkers { get; set; } = new();
	}
}