using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    unsafe class CvModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal fixed byte lfos[Model.LfoCount * LfoModel.Native.Size];
            internal fixed byte envs[Model.EnvCount * EnvModel.Native.Size];
        }
    }
}