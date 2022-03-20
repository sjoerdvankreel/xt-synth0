#include <DSP/Synth/VoiceDSP.hpp>

namespace Xts {

VoiceDSP::
VoiceDSP(SynthModel const* model, int oct, UnitNote note, float velocity, float bpm, float rate):
_cv(& model->cv, velocity, bpm, rate),
_amp(&model->amp, velocity),
_audio(&model->audio, oct, note, rate) {}

} // namespace Xts