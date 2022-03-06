﻿using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum ModSource
    {
        Velocity,
        Env1, Env2, Env3,
        LFO1, LFO2, LFO3
    }

    unsafe class ModModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int target;
            internal int amount;
            internal int source;
            internal int pad__;
        };

        internal static string[] ModSourceNames = { "Velo", "Env1", "Env2", "Env3", "LFO1", "LFO2", "LFO3" };
    }
}