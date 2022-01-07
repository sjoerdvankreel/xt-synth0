#ifndef XTS_SIZE_CHECKS_HPP
#define XTS_SIZE_CHECKS_HPP

#include "SynthModel.hpp"
#include "SequencerModel.hpp"

#define XTS_COMBINE_(X,Y) X##Y
#define XTS_COMBINE(X,Y) XTS_COMBINE_(X,Y)
#define XTS_CHECK_SIZE(type, expected) \
CheckSize<sizeof(type), expected> XTS_COMBINE(Check, __LINE__)

namespace Xts {

template<int Actual, int Expected>
struct CheckSize { static_assert(Actual == Expected); };

XTS_CHECK_SIZE(Param, 16);
XTS_CHECK_SIZE(UnitModel, 32);
XTS_CHECK_SIZE(SynthModel, 496);
XTS_CHECK_SIZE(GlobalModel, 16);

XTS_CHECK_SIZE(PatternFx, 8);
XTS_CHECK_SIZE(EditModel, 16);
XTS_CHECK_SIZE(PatternKey, 16);
XTS_CHECK_SIZE(PatternRow, 88);
XTS_CHECK_SIZE(PatternModel, 22528);
XTS_CHECK_SIZE(SequencerModel, 22544);

} // namespace Xts
#endif // XTS_SIZE_CHECKS_HPP