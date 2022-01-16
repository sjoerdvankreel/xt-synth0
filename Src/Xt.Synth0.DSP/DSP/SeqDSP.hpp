#ifndef XTS_SEQ_DSP_HPP
#define XTS_SEQ_DSP_HPP

#include "SynthDSP.hpp"
#include "PatternDSP.hpp"
#include "../Model/SeqModel.hpp"
#include "../Model/SynthModel.hpp"
#include <cstdint>

namespace Xts {

inline const int MaxVoices = 128;

struct SeqOutput
{
  float l; 
  float r;
};

struct XTS_ALIGN SeqState
{
  float rate;
  int32_t voices;
  XtsBool clip;
  int32_t frames;
  int32_t currentRow;
  int32_t pad__;
  int64_t streamPosition;
  float* buffer;
  SynthModel* synth;
  SeqModel const* seq;
  SeqState() = default;
};

class SeqDSP
{
  int _voicesUsed = 0;
  int _previousRow = -1;
  double _rowFactor = 0.0;
  PatternDSP const _pattern;

  int _voiceKeys[MaxVoices];
  SynthDSP _voiceDsps[MaxVoices];
  int64_t _voicesStarted[MaxVoices];
  SynthModel _voiceModels[MaxVoices];

public:
  void Init(SeqState& state);
  void ProcessBuffer(SeqState& state);

private:
  bool RowUpdated(int currentRow);
  bool UpdateRow(SeqState& state);
  int TakeVoice(int key, int64_t pos);
  void ReleaseVoice(int key, int voice);
  void Next(SeqState& state, SeqOutput& output);
};

} // namespace Xts
#endif // XTS_SEQ_DSP_HPP