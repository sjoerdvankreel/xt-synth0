#ifndef XTS_MODEL_HPP
#define XTS_MODEL_HPP

namespace Xts {

constexpr int MaxFxs = 3;
constexpr int MaxLpb = 16;
constexpr int MaxKeys = 4;
constexpr int MaxRows = 32;
constexpr int MaxPatterns = 8;
constexpr int TotalRows = MaxPatterns * MaxRows;

} // namespace Xts
#endif // XTS_MODEL_HPP