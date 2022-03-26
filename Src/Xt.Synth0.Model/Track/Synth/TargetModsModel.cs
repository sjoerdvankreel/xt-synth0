using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe static class TargetModsModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal TargetModModel.Native mod1;
            internal TargetModModel.Native mod2;
        };
    }
}