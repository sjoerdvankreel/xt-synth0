using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe static class SourceTargetModsModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal SourceTargetModModel.Native mod1;
            internal SourceTargetModModel.Native mod2;
        };
    }
}