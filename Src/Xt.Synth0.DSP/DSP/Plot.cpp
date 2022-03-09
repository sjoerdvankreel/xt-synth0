#include <DSP/Plot.hpp>
#include <DSP/Param.hpp>
#include <DSP/EnvSample.hpp>

#include <cassert>
#include <iomanip>
#include <sstream>
#include <algorithm>

#define MIN_HOLD_MS 1.0f
#define MAX_HOLD_MS 3000.0f

namespace Xts {

static constexpr wchar_t UnicodePi = L'\u03C0';

/*static*/ std::wstring
VSplitMarker(float val, float max)
{
  float absval = std::fabs(val);
  std::wstring result = val == 0.0f ? L"" : val > 0.0f ? L"+" : L"-";
  if(max >= 10)
  { 
    int ival = static_cast<int>(std::roundf(absval));
    return result + std::to_wstring(ival);
  }
  std::wstringstream str;
  str << std::fixed << std::setprecision(1) << absval;
  return result + str.str();
}

/*static*/ std::vector<VSplit>
BiVSPlits = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"+0.5" },
  { 0.0f, L"0.0" },
  { 0.5f, L"-0.5" },
  { 1.0f, L"-1.0" }
};

/*static*/ std::vector<VSplit>
UniVSPlits = {
  { 0.0f, L"1.0" },
  { 0.25f, L".75" },
  { 0.5f, L"0.5" },
  { 0.75f, L".25" },
  { 1.0f, L"0.0" }
};

/*static*/ std::vector<VSplit>
StereoVSPlits = {
  { -1.0f, L"+1.0" },
  { -0.5f, L"L" },
  { 0.0f, L"-/+1" },
  { 0.5f, L"R" },
  { 1.0f, L"-1.0" }
};

/*static*/ std::vector<VSplit>
MakeBiVSplits(float max)
{
  std::vector<VSplit> result;
  result.emplace_back(-1.0f, VSplitMarker(max, max));
  result.emplace_back(-0.5f, VSplitMarker(max / 2.0f, max));
  result.emplace_back(-0.0f, L"0");
  result.emplace_back(0.5f, VSplitMarker(-max / 2.0f, max));
  result.emplace_back(1.0f, VSplitMarker(-max, max));
  return result;
}

/*static*/ std::wstring
FormatEnv(EnvStage stage)
{
  switch (stage)
  {
  case EnvStage::Attack: return L"A";
  case EnvStage::Decay: return L"D";
  case EnvStage::Sustain: return L"S";
  case EnvStage::Release: return L"R";
  case EnvStage::Delay: return L"D";
  case EnvStage::Hold: return L"H";
  case EnvStage::End: return L"";
  default: assert(false); return L"";
  }
}

static void
SpectrumHSplits(std::vector<HSplit>& hSplits)
{
  hSplits.clear();
  for (int oct = 0; oct < 12; oct++)
  {
    std::wstring marker = oct >= 2 ? std::to_wstring(oct - 2) : L"";
    hSplits.emplace_back(HSplit(oct * 12, marker));
  }
  hSplits.emplace_back(HSplit(143, L""));
}

static void
SpectrumVSplitsMono(std::vector<VSplit>& vSplits)
{
  vSplits.clear();
  vSplits.emplace_back(1.0f - (1.0f / 1.0f), L"1.0");
  vSplits.emplace_back(1.0f - (1.0f / 2.0f), L".50");
  vSplits.emplace_back(1.0f - (1.0f / 4.0f), L".25");
  vSplits.emplace_back(1.0f - (1.0f / 8.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 32.0f), L"");
  vSplits.emplace_back(1.0f, L"0.0");
}

static void
SpectrumVSplitsStereo(std::vector<VSplit>& vSplits)
{
  vSplits.clear();
  vSplits.emplace_back(1.0f - (1.0f / 1.0f), L"1.0");
  vSplits.emplace_back(1.0f - (3.0f / 4.0f), L".50");
  vSplits.emplace_back(1.0f - (5.0f / 8.0f), L".25");
  vSplits.emplace_back(1.0f - (9.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f - (1.0f / 2.0f), L"0/1");
  vSplits.emplace_back(1.0f - (1.0f / 4.0f), L".50");
  vSplits.emplace_back(1.0f - (1.0f / 8.0f), L".25");
  vSplits.emplace_back(1.0f - (1.0f / 16.0f), L"");
  vSplits.emplace_back(1.0f, L"0.0");
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

void
PeriodicPlot::RenderCore(PlotInput const& input, PlotOutput& output)
{
  float max = 1.0f;
  auto params = Params();
  Init(input.bpm, input.rate);
  InitPeriodic(this, input, output);
  Init(input.bpm, output.rate);
  float length = (output.rate * params.periods / output.frequency) + 1.0f;
  int samples = static_cast<int>(std::ceilf(input.spectrum? output.rate: length));
  
  for (int i = 0; i < samples; i++)
  {
    float sample = Next();
    max = std::max(max, std::fabs(sample));
    output.lSamples->push_back(sample);
  }

  output.hSplits->emplace_back(samples, L"");
  for (int i = 0; i < params.periods * 2; i++)
    output.hSplits->emplace_back(samples * i / (params.periods * 2), std::to_wstring(i) + UnicodePi);
  if (!params.autoRange)
  {
    assert(max <= 1.0f);
    *(output.vSplits) = params.bipolar ? BiVSPlits : UniVSPlits;
    return;
  }

  assert(params.bipolar);
  for (int i = 0; i < samples; i++) (*output.lSamples)[i] /= max;
  *output.vSplits = MakeBiVSplits(max);
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
  if(output.spectrum || !params.allowResample) return;
  float holdSamples = Param::TimeSamplesF(hold, input.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  output.rate = input.rate * input.pixels / (holdSamples + plot->ReleaseSamples(input.bpm, input.rate));
}

void 
StagedPlot::RenderCore(PlotInput const& input, int hold, PlotOutput& output)
{
  Init(input.bpm, input.rate);
  InitStaged(this, input, hold, output);
  Init(input.bpm, output.rate);

  int h = 0;
  int i = 0;
  float holdSamples = Param::TimeSamplesF(hold, output.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  while ((!output.spectrum && !End()) || (output.spectrum && i < static_cast<int>(output.rate)))
  {
    if (!output.spectrum && h++ == static_cast<int>(holdSamples))
      output.hSplits->emplace_back(i, FormatEnv(Release().stage));
    Next();
    float l = Left();
    output.clip |= Clip(l);
    output.lSamples->push_back(l);
    float r = output.stereo ? Right() : 0.0f;
    output.clip |= Clip(r);
    output.rSamples->push_back(r);
    if (i == 0 || EnvOutput().switchedStage)
      output.hSplits->emplace_back(i, FormatEnv(EnvOutput().stage));
    i++;
  }
}

} // namespace Xts