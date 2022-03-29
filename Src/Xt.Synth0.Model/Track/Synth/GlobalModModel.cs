using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe static class GlobalModModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int amount;
            internal int target;
        };
    }
}