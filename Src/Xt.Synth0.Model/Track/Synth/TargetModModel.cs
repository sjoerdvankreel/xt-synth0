using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe static class TargetModModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal ModModel.Native mod;
            internal int target;
            internal int pad__;
        };
    }
}