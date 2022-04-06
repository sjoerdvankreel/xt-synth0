using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum VoiceModSource
    {
        Velocity,
        Env1, Env2, Env3,
        LFO1, LFO2, 
        GlobalLFO, GlobalLFOHold
    }

    unsafe static class VoiceModModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int amount;
            internal int source;
        };

        internal static readonly string[] ModSourceNames = { "Velo", "Env1", "Env2", "Env3", "LFO1", "LFO2", "LFO3", "LFO3H" };
    }
}