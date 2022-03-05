using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
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
    }
}