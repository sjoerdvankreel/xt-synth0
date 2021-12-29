#ifndef XTS_AUDIO_MODEL_H
#define XTS_AUDIO_MODEL_H

#include "../XtsDefines.h"

typedef void (XT_CALL *
XtsAudioModelRowUpdated)(void);

typedef struct XtsAudioModel
{
  XtsAudioModelRowUpdated rowUpdated;
  int currentRow;
};

#endif // XTS_AUDIO_MODEL_H