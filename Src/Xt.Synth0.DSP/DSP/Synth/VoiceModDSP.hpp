#ifndef XTS_DSP_SYNTH_VOICE_MOD_DSP_HPP
#define XTS_DSP_SYNTH_VOICE_MOD_DSP_HPP

#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/Modulate.hpp>
#include <Model/Synth/VoiceModModel.hpp>
#include <cstdint>

namespace Xts {

class VoiceModDSP
{
  float _amount;
  CvSample _output;
  VoiceModModel const* _model;
public:
  CvSample Next(struct CvState const& cv);
  float Amount() const { return _amount; }
  CvSample Output() const { return _output; }
public:
  float Modulate(CvSample carrier) const
  { return Xts::Modulate(carrier, Output(), Amount()); }
public:
  VoiceModDSP() = default;
  VoiceModDSP(VoiceModModel const* model) : _model(model) {}
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_MOD_DSP_HPP