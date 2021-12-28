#define XT_CALL __stdcall
#define XT_EXPORT __declspec(dllexport)

XT_EXPORT float XT_CALL
xts0_unit_additive(float phase)
{
  return phase;
}