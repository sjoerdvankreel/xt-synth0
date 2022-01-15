#ifndef XTS_SIZE_CHECKS_HPP
#define XTS_SIZE_CHECKS_HPP

#include "SeqModel.hpp"
#include "SynthModel.hpp"

#define XTS_COMBINE_(X,Y) X##Y
#define XTS_COMBINE(X,Y) XTS_COMBINE_(X,Y)
#define XTS_CHECK_SIZE(type, expected) \
CheckSize<sizeof(type), expected> XTS_COMBINE(Check, __LINE__)

namespace Xts {

template<int Actual, int Expected>
struct CheckSize { static_assert(Actual == Expected); };

XTS_CHECK_SIZE(EnvModel, 40);
XTS_CHECK_SIZE(PlotModel, 8);
XTS_CHECK_SIZE(UnitModel, 56);
XTS_CHECK_SIZE(AutoParam, 16);
XTS_CHECK_SIZE(GlobalModel, 16);
XTS_CHECK_SIZE(SynthModel, 1232);

XTS_CHECK_SIZE(PatternFx, 8);
XTS_CHECK_SIZE(EditModel, 24);
XTS_CHECK_SIZE(PatternKey, 16);
XTS_CHECK_SIZE(PatternRow, 88);
XTS_CHECK_SIZE(SeqModel, 22552);
XTS_CHECK_SIZE(PatternModel, 22528);

} // namespace Xts
#endif // XTS_SIZE_CHECKS_HPP