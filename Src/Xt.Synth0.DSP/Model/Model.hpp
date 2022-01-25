#ifndef XTS_MODEL_HPP
#define XTS_MODEL_HPP

#include <cstdint>

#define XTS_ALIGN alignas(8)
#define XTS_COMBINE_(X,Y) X##Y
#define XTS_COMBINE(X,Y) XTS_COMBINE_(X,Y)

#define XTS_CHECK_SIZE(type, expected) \
CheckSize<sizeof(type), expected> \
XTS_COMBINE(Check, XTS_COMBINE(type, __LINE__))

typedef int32_t XtsBool;
constexpr XtsBool XtsTrue = 1;
constexpr XtsBool XtsFalse = 0;

namespace Xts {

constexpr int LfoCount = 2;
constexpr int EnvCount = 3;
constexpr int UnitCount = 3;
constexpr int ParamCount = 132;

constexpr int MaxFxs = 3;
constexpr int MaxLpb = 16;
constexpr int MaxKeys = 4;
constexpr int MaxRows = 32;
constexpr int MaxPatterns = 8;
constexpr int TotalRows = MaxPatterns * MaxRows;

namespace {
template<int Actual, int Expected>
struct CheckSize { static_assert(Actual == Expected); };
} // namespace anonymous

} // namespace Xts
#endif // XTS_MODEL_HPP