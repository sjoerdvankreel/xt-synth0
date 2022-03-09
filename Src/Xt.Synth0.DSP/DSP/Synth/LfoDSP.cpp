#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Plot.hpp>
#include <DSP/Param.hpp>
#include <Model/SynthModel.hpp>

#include <cmath>
#include <memory>
#include <cassert>

#define MIN_FREQ_HZ 0.1f
#define MAX_FREQ_HZ 20.0f

namespace Xts {

static float
Frequency(LfoModel const& model, float bpm, float rate)
{
  if (model.sync) return rate / Param::StepSamplesF(model.step, bpm, rate);
  return Param::Frequency(model.frequency, MIN_FREQ_HZ, MAX_FREQ_HZ);
}

class LfoPlot : public CycledPlot
{
  LfoDSP _dsp;
  LfoModel const* _model;
public:
  LfoPlot(LfoModel const* model): _model(model) {}

  int Cycles() const { return 1; }
  bool AutoRange() const { return false; }
  float Next() { return _dsp.Next().value; }
  bool AllowResample() const { return true; }
  bool Bipolar() const { return _model->unipolar == 0; }  
  void Init(float bpm, float rate) { new(&_dsp) LfoDSP(_model, bpm, rate); }
  float Frequency(float bpm, float rate) const { return Xts::Frequency(*_model, bpm, rate); }
};

CvSample
LfoDSP::Next()
{
  _output.value = 0.0f;
  if (!_model->on) return Output();
  _output.value = Generate();
  _phase += _increment;
  _phase -= std::floor(_phase);
  return Output().Sanity();
}

LfoDSP::
LfoDSP(LfoModel const* model, float bpm, float rate) :
LfoDSP()
{
  _phase = 0.0;
  _model = model;
  _output.bipolar = model->unipolar == 0;
  _base = model->unipolar == 0 ? 0.0f: 0.5f;
  _increment = Frequency(*model, bpm, rate) / rate;
  _factor = (model->invert ? -1.0f : 1.0f) * (1.0f - _base);
}

void
LfoDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  int base = static_cast<int>(PlotType::LFO1);
  int type = static_cast<int>(model.plot.type);
  LfoModel const* lfo = &model.cv.lfos[type - base];
  if (lfo->on) LfoPlot(lfo).Render(input, output);
}

float
LfoDSP::Generate() const
{
	float phase = static_cast<float>(_phase);
	switch (_model->type)
	{
	case LfoType::Tri: break;
	case LfoType::Saw: return _base + _factor * (phase * 2.0f - 1.0f);
  case LfoType::Sqr: return _base + _factor * (phase < 0.5f ? 1.0f : -1.0f);
  case LfoType::Sin: return _base + _factor * std::sinf(phase * 2.0f * PIF);
	default: assert(false); return 0.0f;
	}
	float tri = phase < 0.25f ? phase : phase < 0.75f ? 0.5f - phase : (phase - 0.75f) - 0.25f;
	return _base + _factor * tri * 4.0f;
}

} // namespace Xts