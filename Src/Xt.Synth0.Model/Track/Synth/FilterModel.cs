using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum FilterType { StateVar, Comb };
    public enum PassType { LPF, HPF, BPF, BSF };

    public enum FilterModTarget
    {
        Frequency,
        Resonance,
        CombMinGain,
        CombPlusGain,
        CombMinDelay,
        CombPlusDelay
    };

    public unsafe static class FilterModel
    {
        internal const double MinFreqHz = 20.0;
        internal const double MaxFreqHz = 10000.0;
        internal const double CombMaxDelayMs = 5.0;
        internal const double CombMinDelayMs = 0.0;

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int on;
            internal int type;

            internal int combMinGain;
            internal int combPlusGain;
            internal int combMinDelay;
            internal int combPlusDelay;

            internal int passType;
            internal int resonance;
            internal int frequency;
            internal int pad__;
        };

        internal static readonly string[] TypeNames = { "StVar", "Comb" };
        internal static readonly string[] TargetNames = { "Freq", "Res", "Gain-", "Gain+", "Delay-", "Delay+" };
    }
}