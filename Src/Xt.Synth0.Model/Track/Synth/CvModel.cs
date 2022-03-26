using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    static unsafe class CvModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal fixed byte lfos[SynthConfig.VoiceLfoCount * LfoModel.Native.Size];
            internal fixed byte envs[SynthConfig.VoiceEnvCount * EnvModel.Native.Size];
        }
    }
}