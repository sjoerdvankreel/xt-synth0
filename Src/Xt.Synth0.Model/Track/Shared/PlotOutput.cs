using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public static class PlotOutput
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            public float min;
            public float max;
            public float rate;
            public float frequency;
            public int clip;
            public int stereo;
            public int spectrum;
            public int pad__;
        }
    }
}