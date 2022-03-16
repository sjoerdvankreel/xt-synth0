using System.Runtime.InteropServices;
using System.Text;

namespace Xt.Synth0.Model
{
    public unsafe static class PlotResult
    {
        static string FromWideChar(ushort* text)
        {
            int i = 0;
            while (text[i] != 0) i++;
            return Encoding.Unicode.GetString((byte*)text, i * 2);
        }

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

            public string VerticalText(int i) { return FromWideChar(verticalTexts[i]); }
            public string HorizontalText(int i) { return FromWideChar(horizontalTexts[i]); }
        }
    }
}