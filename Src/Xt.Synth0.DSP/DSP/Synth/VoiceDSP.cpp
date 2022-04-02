#include <DSP/Synth/VoiceDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

namespace Xts {

VoiceDSP::
VoiceDSP(SynthModel const* model, int oct, NoteType note, float velocity, float bpm, float rate):
_cv(&model->voice.cv, velocity, bpm, rate),
_amp(&model->voice.amp, velocity),
_audio(&model->voice.audio, oct, note, rate) {}

} // namespace Xts