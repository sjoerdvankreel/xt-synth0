#include <DSP/Shared/Param.hpp>
#include <DSP/Synth/LfoDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <cmath>
#include <memory>
#include <cassert>

#define MIN_FREQ_HZ 0.1f
#define MAX_FREQ_HZ 20.0f

namespace Xts {

PeriodicParams
LfoPlot::Params() const
{
  PeriodicParams result;
  result.periods = 1;
  result.autoRange = false;
  result.allowResample = true;
  result.bipolar = _model->unipolar == 0;
  return result;
}

void
LfoPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  LfoModel const* lfo = &model.global.lfo;
  if(model.global.plot.type != PlotType::GlobalLFO)
  {
    int base = static_cast<int>(PlotType::LFO1);
    int type = static_cast<int>(model.global.plot.type);
    lfo = &model.voice.cv.lfos[type - base];
  }
  if (lfo->on) LfoPlot(lfo).DoRender(input, state);
}

CvSample
LfoDSP::Next()
{
  _output.value = 0.0f;
  _output.bipolar = _model->unipolar == 0;
  if (!_model->on) return Output();
  float frequency = Frequency(*_model, _bpm, _rate);
  _output.value = Generate(frequency);
  _phase += frequency / _rate;
  _phase -= std::floor(_phase);
  return Output().Sanity();
}

LfoDSP::
LfoDSP(LfoModel const* model, float bpm, float rate) :
LfoDSP()
{
  _bpm = bpm;
  _rate = rate;
  _phase = 0.0;
  _model = model;
}

float
LfoDSP::Frequency(LfoModel const& model, float bpm, float rate)
{
  if (model.sync) return rate / Param::StepSamplesF(model.step, bpm, rate);
  return Param::Frequency(model.frequency, MIN_FREQ_HZ, MAX_FREQ_HZ);
}

float
LfoDSP::Generate(float frequence) const
{
  float phase = static_cast<float>(_phase);
  float base = _model->unipolar == 0 ? 0.0f : 0.5f;
  float factor = (_model->invert ? -1.0f : 1.0f) * (1.0f - base);
	switch (_model->type)
	{
	case LfoType::Tri: break;
	case LfoType::Saw: return base + factor * (phase * 2.0f - 1.0f);
  case LfoType::Sqr: return base + factor * (phase < 0.5f ? 1.0f : -1.0f);
  case LfoType::Sin: return base + factor * std::sinf(phase * 2.0f * PIF);
	default: assert(false); return 0.0f;
	}
	float tri = phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : (phase - 0.75f) - 0.25f;
	return base + factor * tri * 4.0f;
}

} // namespace Xts