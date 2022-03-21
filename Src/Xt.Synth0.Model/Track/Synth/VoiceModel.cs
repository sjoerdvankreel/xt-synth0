using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class VoiceModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            internal CvModel.Native cv;
            internal AmpModel.Native amp;
            internal AudioModel.Native audio;
        }
    }
}