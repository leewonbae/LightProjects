#include <cstdint>
namespace commons {
    enum Protocols : uint16_t {
        NONE = 0,
        C_TO_S_MESSAGE = 1,
        S_TO_C_MESSAGE = 2
    };

    enum PacketErrorCode : uint16_t {
        NONE,
    };
}