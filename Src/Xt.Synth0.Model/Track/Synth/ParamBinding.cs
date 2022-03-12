using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe class ParamBinding
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            internal fixed byte @params[SynthConfig.ParamCount * 8];
        }
    }
}