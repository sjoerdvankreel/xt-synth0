#include <DSP/Synth/VoiceDSP.hpp>

namespace Xts {

VoiceDSP::
VoiceDSP(int oct, UnitNote note, float velocity, float bpm, float rate):
_model(),
_binding(std::vector<int*>(XTS_SYNTH_PARAM_COUNT)),
_amp(&_model.voice.amp, velocity),
_cv(&_model.voice.cv, velocity, bpm, rate),
_audio(&_model.voice.audio, oct, note, rate) {}

} // namespace Xts