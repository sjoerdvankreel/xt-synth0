#ifndef XTS_TRACK_CONSTANTS_HPP
#define XTS_TRACK_CONSTANTS_HPP

#define XTS_ALIGN alignas(8)

namespace Xts { namespace TrackConstants {

constexpr int MinOct = 0;
constexpr int MaxOct = 9;

constexpr int UnitCount = 3;
constexpr int ParamCount = 24;

constexpr int BeatRows = 4;
constexpr int MaxFxCount = 3;
constexpr int MaxKeyCount = 4;
constexpr int PatternRows = 32;
constexpr int PatternCount = 8;
constexpr int TotalRowCount = PatternCount * PatternRows;

} } // namespace Xts::TrackConstants
#endif // XTS_TRACK_CONSTANTS_HPP