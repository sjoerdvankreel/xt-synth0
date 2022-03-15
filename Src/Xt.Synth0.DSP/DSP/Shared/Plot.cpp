#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Spectrum.hpp>
#include <DSP/Shared/EnvSample.hpp>

#include <cassert>
#include <cassert>
#include <iomanip>
#include <sstream>
#include <algorithm>

#define MIN_HOLD_MS 1.0f
#define MAX_HOLD_MS 3000.0f

namespace Xts {

static std::wstring
MarkVertical(float val, float max);

static constexpr wchar_t UnicodePi = L'\u03C0';

static std::vector<VerticalMarker>
BipolarMarkers = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"+0.5" },
  { 0.0f, L"0.0" },
  { 0.5f, L"-0.5" },
  { 1.0f, L"-1.0" }
};

static std::vector<VerticalMarker>
UnipolarMarkers = {
  { 0.0f, L"1.0" },
  { 0.25f, L".75" },
  { 0.5f, L"0.5" },
  { 0.75f, L".25" },
  { 1.0f, L"0.0" }
};

static std::vector<VerticalMarker>
StereoMarkers = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"L" },
  { 0.0f, L"-/+1" },
  { 0.5f, L"R" },
  { 1.0f, L"-1.0" }
};

static std::wstring
FormatEnv(EnvStage stage)
{
  switch (stage)
  {
  case EnvStage::End: return L"";
  case EnvStage::Hold: return L"H";
  case EnvStage::Delay: return L"D";
  case EnvStage::Decay: return L"D";
  case EnvStage::Attack: return L"A";
  case EnvStage::Sustain: return L"S";
  case EnvStage::Release: return L"R";
  default: assert(false); return L"";
  }
}

static std::vector<VerticalMarker>
MakeBipolarMarkers(float max)
{
  std::vector<VerticalMarker> result;
  result.emplace_back(-1.0f, MarkVertical(max, max));
  result.emplace_back(-0.5f, MarkVertical(max / 2.0f, max));
  result.emplace_back(-0.0f, L"0");
  result.emplace_back(0.5f, MarkVertical(-max / 2.0f, max));
  result.emplace_back(1.0f, MarkVertical(-max, max));
  return result;
}

static std::wstring
MarkVertical(float val, float max)
{
  float absval = std::fabs(val);
  std::wstring result = val == 0.0f ? L"" : val > 0.0f ? L"+" : L"-";
  if(max >= 10) return result + std::to_wstring(static_cast<int>(std::roundf(absval)));
  std::wstringstream str;
  str << std::fixed << std::setprecision(1) << absval;
  return result + str.str();
}

static void
ApplyAutoRange(PlotData& data, float max)
{
  for (size_t i = 0; i < data.left.size(); i++) data.left[i] /= max;
  data.vertical = MakeBipolarMarkers(max);
}

static void
InitPeriodic(PeriodicPlot* plot, PlotInput const& input, PlotOutput& output, PlotData& data)
{
  auto params = plot->Params();
  output.max = 1.0f;
  output.clip = false;
  output.stereo = false;
  output.rate = input.rate;
  output.spectrum = input.spectrum;
  output.min = params.bipolar ? -1.0f : 0.0f;
  output.frequency = plot->Frequency(input.bpm, input.rate);
  data.vertical = params.bipolar ? BipolarMarkers : UnipolarMarkers;
  if(input.spectrum || !params.allowResample) return;
  output.rate = std::min(input.rate, output.frequency * input.pixels / params.periods);
}

static void
InitStaged(StagedPlot* plot, PlotInput const& input, int hold, PlotOutput& output, PlotData& data)
{
  auto params = plot->Params();
  output.max = 1.0f;
  output.clip = false;
  output.rate = input.rate;
  output.stereo = params.stereo;
  output.min = params.bipolar ? -1.0f : 0.0f;
  output.spectrum = input.spectrum != 0 && params.allowSpectrum;
  data.vertical = params.stereo ? StereoMarkers : params.bipolar ? BipolarMarkers : UnipolarMarkers;
  if (output.spectrum || !params.allowResample) return;
  float holdSamples = Param::TimeSamplesF(hold, input.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  output.rate = input.rate * input.pixels / (holdSamples + plot->ReleaseSamples(input.bpm, input.rate));
}

void
PeriodicPlot::RenderCore(PlotInput const& input, int hold, PlotOutput& output, PlotData& data)
{
  float max = 1.0f;
  auto params = Params();
  Init(input.bpm, input.rate);
  InitPeriodic(this, input, output, data);
  Init(input.bpm, output.rate);

  float length = (output.rate * params.periods / output.frequency) + 1.0f;
  int samples = static_cast<int>(std::ceilf(input.spectrum ? output.rate : length));
  int halfPeriod = samples / (params.periods * 2);
  data.horizontal.emplace_back(samples - 1, L"");
  for (int i = 0; i < samples; i++)
  {
    float sample = Next();
    max = std::max(max, std::fabs(sample));
    data.left.push_back(sample);
    if (i / halfPeriod < params.periods * 2 && i % halfPeriod == 0)
      data.horizontal.emplace_back(i, std::to_wstring(i / halfPeriod) + UnicodePi);
  }
  if (params.autoRange) ApplyAutoRange(data, max);
}

void 
StagedPlot::RenderCore(PlotInput const& input, int hold, PlotOutput& output, PlotData& data)
{
  int h = 0;
  int i = 0;
  bool clip = false;
  bool done = false;
  Init(input.bpm, input.rate);
  InitStaged(this, input, hold, output, data);
  Init(input.bpm, output.rate);

  float holdSamples = Param::TimeSamplesF(hold, output.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  while (!done)
  {
    if (!output.spectrum && h++ == static_cast<int>(holdSamples)) 
      data.horizontal.emplace_back(i, FormatEnv(Release().stage));
    Next();
    data.left.push_back(Clip(Left(), clip));
    data.right.push_back(Clip(Right(), clip));
    if (i == 0 || EnvOutput().switchedStage) 
      data.horizontal.emplace_back(i, FormatEnv(EnvOutput().stage));
    done |= !output.spectrum && End();
    done |= output.spectrum != 0 && i == static_cast<int>(output.rate);
    i++;
  }
  output.clip = clip;
}

void
Plot::DoRender(PlotState& state)
{
  state.data = PlotData();
  state.output = PlotOutput();
  state.result = PlotResult();
  state.scratch = PlotScratch();
  RenderCore(state.input, state.hold, state.output, state.data);
  assert(state.output.rate <= state.input.rate);
  if (state.output.spectrum) TransformToSpectrum(state.output, state.data, state.scratch);
  state.result.left = state.data.left.data();
  state.result.right = state.data.right.data();
  state.result.sampleCount = static_cast<int32_t>(state.data.left.size());
  state.result.verticalCount = static_cast<int32_t>(state.data.vertical.size());
  state.result.horizontalCount = static_cast<int32_t>(state.data.horizontal.size());
  assert(state.result.sampleCount == state.data.right.size() || state.data.right.size() == 0);
  for (size_t i = 0; i < state.data.vertical.size(); i++)
  {
    state.scratch.verticalTexts.push_back(state.data.vertical[i].text.c_str());
    state.scratch.verticalPositions.push_back(state.data.vertical[i].position);
  }
  for (size_t i = 0; i < state.data.horizontal.size(); i++)
  {
    state.scratch.horizontalTexts.push_back(state.data.horizontal[i].text.c_str());
    state.scratch.horizontalPositions.push_back(state.data.horizontal[i].position);
  }
  state.result.verticalTexts = state.scratch.verticalTexts.data();
  state.result.horizontalTexts = state.scratch.horizontalTexts.data();
  state.result.verticalPositions = state.scratch.verticalPositions.data();
  state.result.horizontalPositions = state.scratch.horizontalPositions.data();
}

} // namespace Xts