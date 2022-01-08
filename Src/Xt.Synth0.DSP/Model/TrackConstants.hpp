#ifndef XTS_TRACK_CONSTANTS_HPP
#define XTS_TRACK_CONSTANTS_HPP

#define XTS_ALIGN alignas(8)

namespace Xts { namespace TrackConstants {

constexpr int MinOct = 0;
constexpr int MaxOct = 9;

constexpr int UnitCount = 3;
constexpr int ParamCount = 27;

constexpr int MaxFxs = 3;
constexpr int MaxLpb = 16;
constexpr int MaxKeys = 4;
constexpr int MaxRows = 32;
constexpr int MaxPatterns = 8;
constexpr int TotalRows = MaxPatterns * MaxRows;

} } // namespace Xts::TrackConstants
#endif // XTS_TRACK_CONSTANTS_HPP