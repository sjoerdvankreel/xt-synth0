#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/EnvSample.hpp>

#include <cassert>
#include <iomanip>
#include <sstream>
#include <algorithm>

#define MIN_HOLD_MS 1.0f
#define MAX_HOLD_MS 3000.0f

namespace Xts {

static std::wstring
VSplitMarker(float val, float max);

static constexpr wchar_t UnicodePi = L'\u03C0';

static std::vector<VSplit>
BipolarVSPlits = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"+0.5" },
  { 0.0f, L"0.0" },
  { 0.5f, L"-0.5" },
  { 1.0f, L"-1.0" }
};

static std::vector<VSplit>
UnipolarVSPlits = {
  { 0.0f, L"1.0" },
  { 0.25f, L".75" },
  { 0.5f, L"0.5" },
  { 0.75f, L".25" },
  { 1.0f, L"0.0" }
};

static std::vector<VSplit>
StereoVSPlits = {
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

static std::vector<VSplit>
MakeBipolarVSplits(float max)
{
  std::vector<VSplit> result;
  result.emplace_back(-1.0f, VSplitMarker(max, max));
  result.emplace_back(-0.5f, VSplitMarker(max / 2.0f, max));
  result.emplace_back(-0.0f, L"0");
  result.emplace_back(0.5f, VSplitMarker(-max / 2.0f, max));
  result.emplace_back(1.0f, VSplitMarker(-max, max));
  return result;
}

static std::wstring
VSplitMarker(float val, float max)
{
  float absval = std::fabs(val);
  std::wstring result = val == 0.0f ? L"" : val > 0.0f ? L"+" : L"-";
  if(max >= 10) return result + std::to_wstring(static_cast<int>(std::roundf(absval)));
  std::wstringstream str;
  str << std::fixed << std::setprecision(1) << absval;
  return result + str.str();
}

static void
InitPeriodic(PeriodicPlot* plot, PlotInput const& input, PlotOutput& output)
{
  auto params = plot->Params();
  output.max = 1.0f;
  output.clip = false;
  output.stereo = false;
  output.rate = input.rate;
  output.spectrum = input.spectrum;
  output.min = params.bipolar ? -1.0f : 0.0f;
  output.frequency = plot->Frequency(input.bpm, input.rate);
  if(input.spectrum || !params.allowResample) return;
  output.rate = std::min(input.rate, output.frequency * input.pixels / params.periods);
}

static void
InitStaged(StagedPlot* plot, PlotInput const& input, int hold, PlotOutput& output)
{
  auto params = plot->Params();
  output.max = 1.0f;
  output.clip = false;
  output.rate = input.rate;
  output.stereo = params.stereo;
  output.spectrum = input.spectrum;
  output.min = params.bipolar ? -1.0f : 0.0f;
  *output.vSplits = params.stereo ? StereoVSPlits : params.bipolar ? BipolarVSPlits : UnipolarVSPlits;
  if (output.spectrum || !params.allowResample) return;
  float holdSamples = Param::TimeSamplesF(hold, input.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  output.rate = input.rate * input.pixels / (holdSamples + plot->ReleaseSamples(input.bpm, input.rate));
}

void 
StagedPlot::RenderCore(PlotInput const& input, int hold, PlotOutput& output)
{
  int h = 0;
  int i = 0;
  bool done = false;
  Init(input.bpm, input.rate);
  InitStaged(this, input, hold, output);
  Init(input.bpm, output.rate);

  float holdSamples = Param::TimeSamplesF(hold, output.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  while (!done)
  {
    if (!output.spectrum && h++ == static_cast<int>(holdSamples)) 
      output.hSplits->emplace_back(i, FormatEnv(Release().stage));
    Next();
    output.lSamples->push_back(Clip(Left(), output.clip));
    output.rSamples->push_back(Clip(Right(), output.clip));
    if (i == 0 || EnvOutput().switchedStage) 
      output.hSplits->emplace_back(i, FormatEnv(EnvOutput().stage));
    done |= !output.spectrum && End();
    done |= output.spectrum && i == static_cast<int>(output.rate);
    i++;
  }
}

void
PeriodicPlot::RenderCore(PlotInput const& input, PlotOutput& output)
{
  float max = 1.0f;
  auto params = Params();
  Init(input.bpm, input.rate);
  InitPeriodic(this, input, output);
  Init(input.bpm, output.rate);

  float length = (output.rate * params.periods / output.frequency) + 1.0f;
  int samples = static_cast<int>(std::ceilf(input.spectrum ? output.rate : length));
  int halfPeriod = samples / (params.periods * 2);
  output.hSplits->emplace_back(samples - 1, L"");
  for (int i = 0; i < samples; i++)
  {
    float sample = Next();
    max = std::max(max, std::fabs(Next()));
    output.lSamples->push_back(sample);
    if(i / halfPeriod < params.periods * 2 && i % halfPeriod == 0)
      output.hSplits->emplace_back(i, std::to_wstring(i / halfPeriod) + UnicodePi);
  }

  *(output.vSplits) = params.bipolar ? BipolarVSPlits : UnipolarVSPlits;
  if (!params.autoRange) return;
  for (int i = 0; i < samples; i++) (*output.lSamples)[i] /= max;
  *output.vSplits = MakeBipolarVSplits(max);
}

} // namespace Xts