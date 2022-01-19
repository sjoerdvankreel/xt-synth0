#ifndef XTS_HPP
#define XTS_HPP

#include "Model/Model.hpp"

#include <vector>
#include <memory>
#include <cstdint>

#define XTS_CALL __stdcall
#define XTS_EXPORT extern "C" __declspec(dllexport)

namespace Xts {
class SeqDSP;
class PlotDSP;
struct SeqModel;
struct SynthModel;
} // namespace Xts

struct XTS_ALIGN SeqState
{
public:
  int64_t pos;
  int32_t row, voices;
  XtsBool clip, exhausted;
public:
  float rate;
  float* buffer;
  int32_t frames;
  Xts::SynthModel* synth;
  Xts::SeqModel const* seq;
public:
  SeqState() = default;
  SeqState(SeqState const&) = delete;
};

struct XTS_ALIGN PlotState
{
public:
  float bpm, pixels;
public:
  float freq, rate;
  XtsBool clip, bipolar;
  std::unique_ptr<std::vector<float>> samples;
  std::unique_ptr<std::vector<int32_t>> splits;
public:
  PlotState(PlotState const&) = delete;
  PlotState():
  splits(std::make_unique<std::vector<int32_t>>()), 
  samples(std::make_unique<std::vector<float>>()) {}
};

XTS_EXPORT SeqState* XTS_CALL XtsSeqStateCreate(void);
XTS_EXPORT PlotState* XTS_CALL XtsPlotStateCreate(void);
XTS_EXPORT Xts::SeqModel* XTS_CALL XtsSeqModelCreate(void);
XTS_EXPORT Xts::SynthModel* XTS_CALL XtsSynthModelCreate(void);

XTS_EXPORT void XTS_CALL XtsSeqStateDestroy(SeqState* state);
XTS_EXPORT void XTS_CALL XtsPlotStateDestroy(PlotState* state);
XTS_EXPORT void XTS_CALL XtsSeqModelDestroy(Xts::SeqModel* model);
XTS_EXPORT void XTS_CALL XtsSynthModelDestroy(Xts::SynthModel* model);

XTS_EXPORT Xts::SeqDSP* XTS_CALL XtsSeqDSPCreate(void);
XTS_EXPORT void XTS_CALL XtsSeqDSPInit(Xts::SeqDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSeqDSPDestroy(Xts::SeqDSP* dsp);
XTS_EXPORT void XTS_CALL XtsSeqDSPRender(Xts::SeqDSP* dsp, SeqState* state);
XTS_EXPORT void XTS_CALL XtsPlotDSPRender(Xts::PlotDSP* dsp, PlotState* state);

#endif // XTS_HPP