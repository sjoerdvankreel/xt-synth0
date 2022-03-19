using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe class ModsModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal ModModel.Native mod1;
            internal ModModel.Native mod2;
        };
    }
}