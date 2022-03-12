using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe class ModModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int source;
            internal int amount;
            internal int target;
            internal int pad__;
        };
    }
}