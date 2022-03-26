using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe class AudioModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal fixed byte units[SynthConfig.VoiceUnitCount * UnitModel.Native.Size];
            internal fixed byte filters[SynthConfig.VoiceFilterCount * VoiceFilterModel.Native.Size];
        }
    }
}