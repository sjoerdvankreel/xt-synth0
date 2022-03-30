#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Shared/Modulate.hpp>
#include <DSP/Synth/MasterDSP.hpp>
#include <Model/Synth/MasterModel.hpp>

namespace Xts {

FloatSample 
MasterDSP::Next(CvSample globalLfo, FloatSample x)
{
  float pan = Xts::Modulate({ Param::Mix(_model->pan), true }, globalLfo, Param::Mix(_model->panLfo));
  float amp = Xts::Modulate({ Param::Level(_model->amp), false }, globalLfo, Param::Mix(_model->ampLfo));
  float panning = BipolarToUnipolar1(pan);
  FloatSample panned = { (1.0f - panning) * amp, panning * amp };
  _output = x * panned;
  return _output.Sanity();
}

} // namespace Xts