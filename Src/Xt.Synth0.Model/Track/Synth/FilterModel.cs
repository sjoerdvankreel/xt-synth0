using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum FilterType { Ladder, StateVar, Comb };
    public enum StateVarPassType { LPF, HPF, BPF, BSF };

    public enum FilterModTarget
    {
        LPHP,
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

            internal int resonance;
            internal int frequency;
            internal int ladderLpHp;
            internal int stateVarPassType;
        };

        internal static readonly string[] TypeNames = { "Lddr", "StVr", "Comb" };
        internal static readonly string[] TargetNames = { "LPHP", "Freq", "Res", "Gain-", "Gain+", "Delay-", "Delay+" };
    }
}