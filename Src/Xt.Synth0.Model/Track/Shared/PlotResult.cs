using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe static class PlotResult
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            public float* left;
            public float* right;
            public int sampleCount;
            public int verticalCount;
            public int horizontalCount;
            public int pad__;
            public float* verticalPositions;
            public int* horizontalPositions;
            public ushort** verticalTexts;
            public ushort** horizontalTexts;
        }
    }
}