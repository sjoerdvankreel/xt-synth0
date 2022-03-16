using System;

namespace Xt.Synth0
{
    static unsafe class BufferConvert
    {
        internal static void To(float* from, IntPtr to, XtSample sample, int frames)
        {
            switch (sample)
            {
                case XtSample.Int16: Int16(from, to, frames); break;
                case XtSample.Int24: Int24(from, to, frames); break;
                case XtSample.Int32: Int32(from, to, frames); break;
                default: throw new InvalidOperationException();
            }
        }

        static void Int32(float* from, IntPtr to, int frames)
        {
            int* samples = (int*)to;
            for (int f = 0; f < frames; f++)
            {
                samples[f * 2] = (int)(from[f * 2] * int.MaxValue);
                samples[f * 2 + 1] = (int)(from[f * 2 + 1] * int.MaxValue);
            }
        }

        static void Int16(float* from, IntPtr to, int frames)
        {
            short* samples = (short*)to;
            for (int f = 0; f < frames; f++)
            {
                samples[f * 2] = (short)(from[f * 2] * short.MaxValue);
                samples[f * 2 + 1] = (short)(from[f * 2 + 1] * short.MaxValue);
            }
        }

        static void Int24(float* from, IntPtr to, int frames)
        {
            byte* samples = (byte*)to;
            for (int f = 0; f < frames; f++)
            {
                int left = (int)(from[f * 2] * int.MaxValue);
                int right = (int)(from[f * 2 + 1] * int.MaxValue);
                samples[f * 6 + 0] = (byte)((left & 0x0000FF00) >> 8);
                samples[f * 6 + 1] = (byte)((left & 0x00FF0000) >> 16);
                samples[f * 6 + 2] = (byte)((left & 0xFF000000) >> 24);
                samples[f * 6 + 3] = (byte)((right & 0x0000FF00) >> 8);
                samples[f * 6 + 4] = (byte)((right & 0x00FF0000) >> 16);
                samples[f * 6 + 5] = (byte)((right & 0xFF000000) >> 24);
            }
        }
    }
}