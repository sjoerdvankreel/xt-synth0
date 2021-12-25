using System;

namespace Xt.Synth0.UI
{
	public class RequestPlotDataEventArgs: EventArgs
	{
		public int Samples { get; set; }
		public float[] Data { get; set; }
	}
}