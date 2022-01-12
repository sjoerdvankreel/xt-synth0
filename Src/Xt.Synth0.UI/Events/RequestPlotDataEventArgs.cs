using System;
using System.Collections.Generic;

namespace Xt.Synth0.UI
{
	public class RequestPlotDataEventArgs : EventArgs
	{
		public int Pixels { get; set; }
		public int SampleRate { get; set; }
		public float Frequency { get; set; }
		public List<int> Splits { get; } = new();
		public List<float> Samples { get; } = new();
	}
}