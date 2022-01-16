#ifndef XTS_TRACK_CONSTANTS_HPP
#define XTS_TRACK_CONSTANTS_HPP

#include <cstdint>

#define XTS_ALIGN alignas(8)

typedef int32_t XtsBool;
inline constexpr XtsBool XtsTrue = 1;
inline constexpr XtsBool XtsFalse = 0;

namespace Xts { namespace TrackConstants {

constexpr int EnvCount = 2;
constexpr int UnitCount = 3;
constexpr int AutoParamCount = 59;

constexpr int MaxFxs = 3;
constexpr int MaxLpb = 16;
constexpr int MaxKeys = 4;
constexpr int MaxRows = 32;
constexpr int MaxPatterns = 8;
constexpr int TotalRows = MaxPatterns * MaxRows;

} } // namespace Xts::TrackConstants
#endif // XTS_TRACK_CONSTANTS_HPP