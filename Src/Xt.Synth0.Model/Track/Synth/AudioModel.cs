using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe class AudioModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal fixed byte units[SynthConfig.UnitCount * UnitModel.Native.Size];
            internal fixed byte filters[SynthConfig.FilterCount * FilterModel.Native.Size];
        }
    }
}