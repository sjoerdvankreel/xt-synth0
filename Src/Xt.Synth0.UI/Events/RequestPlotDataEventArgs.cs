using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
    public unsafe class RequestPlotDataEventArgs : EventArgs
    {
        public float Pixels { get; }
        public PlotOutput.Native* Output { get; set; }
        public PlotResult.Native* Result { get; set; }
        internal RequestPlotDataEventArgs(float pixels) => Pixels = pixels;
    }
}