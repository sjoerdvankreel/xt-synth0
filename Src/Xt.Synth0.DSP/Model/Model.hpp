#ifndef XTS_MODEL_HPP
#define XTS_MODEL_HPP

#include <cstdint>

#define XTS_ALIGN alignas(8)
#define XTS_COMBINE_(X,Y) X##Y
#define XTS_COMBINE(X,Y) XTS_COMBINE_(X,Y)
#define XTS_CHECK_SIZE(type, expected) \
CheckSize<sizeof(type), expected> XTS_COMBINE(Check, __LINE__)

typedef int32_t XtsBool;
inline constexpr XtsBool XtsTrue = 1;
inline constexpr XtsBool XtsFalse = 0;

namespace Xts {

constexpr int EnvCount = 2;
constexpr int UnitCount = 3;
constexpr int ParamCount = 67;

constexpr int MaxFxs = 3;
constexpr int MaxLpb = 16;
constexpr int MaxKeys = 4;
constexpr int MaxRows = 32;
constexpr int MaxPatterns = 8;
constexpr int TotalRows = MaxPatterns * MaxRows;

template<int Actual, int Expected>
struct CheckSize { static_assert(Actual == Expected); };

} // namespace Xts
#endif // XTS_MODEL_HPP