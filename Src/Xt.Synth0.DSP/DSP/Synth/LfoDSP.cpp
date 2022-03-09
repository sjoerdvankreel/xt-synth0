#include <DSP/Synth/LfoDSP.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Param.hpp>

#include <cmath>
#include <cassert>

#define MIN_FREQ_HZ 0.1f
#define MAX_FREQ_HZ 20.0f

namespace Xts {

static void
PlotDSPDestroy(void* dsp)
{ delete static_cast<LfoDSP*>(dsp); }

static float
PlotDSPNext(void* dsp)
{ return static_cast<LfoDSP*>(dsp)->Next().value; }

static void*
PlotDSPCreate(float rate, void* context)
{
  auto state = static_cast<LfoPlotState*>(context);
  return new LfoDSP(state->model, state->input->bpm, rate);
}

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
  _increment = Frequency(*_model, bpm, rate) / rate;
  _factor = (model->invert ? -1.0f : 1.0f) * (1.0f - _base);
}

float
LfoDSP::Frequency(LfoModel const& model, float bpm, float rate)
{
  if (model.sync) return rate / Param::StepSamplesF(model.step, bpm, rate);
  return Param::Frequency(model.frequency, MIN_FREQ_HZ, MAX_FREQ_HZ);
}

void
LfoDSP::Plot(LfoPlotState* state)
{
  if (!state->model->on) return;
  CycledPlotState cycled;
  cycled.cycles = 1;
  cycled.context = state;
  cycled.flags = PlotNone;
  cycled.input = state->input;
  cycled.output = state->output;
  cycled.dspNext = PlotDSPNext;
  cycled.dspCreate = PlotDSPCreate;
  cycled.dspDestroy = PlotDSPDestroy;
  if (state->spectrum) cycled.flags |= PlotSpectrum;
  if (state->model->unipolar == 0) cycled.flags |= PlotBipolar;
  cycled.frequency = Frequency(*state->model, state->input->bpm, state->input->rate);
  PlotDSP::RenderCycled(&cycled);
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