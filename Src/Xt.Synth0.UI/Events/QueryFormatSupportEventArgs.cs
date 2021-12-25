using System;

namespace Xt.Synth0.UI
{
	public class QueryFormatSupportEventArgs : EventArgs
	{
		public bool IsSupported { get; set; }
		public double MinBuffer { get; set; }
		public double MaxBuffer { get; set; }
		public double DefaultBuffer { get; set; }
	}
}