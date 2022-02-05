#ifndef XTS_UTILITY_HPP
#define XTS_UTILITY_HPP

#include <string>
#include <sstream>

namespace Xts {

inline std::string
ToString(float x)
{
  std::ostringstream oss;
  oss << x;
  return oss.str();
}

} // namespace Xts
#endif // XTS_UTILITY_HPP