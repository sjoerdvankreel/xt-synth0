using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe static class GlobalModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            internal LfoModel.Native lfo;
            internal PlotModel.Native plot;
            internal GlobalFilterModel.Native filter;
        }
    }
}