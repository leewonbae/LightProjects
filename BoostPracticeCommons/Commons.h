#include <cstdint>
namespace commons {
    enum Protocols : uint32_t {
        PROTOCOL_NONE = 0,
        C_TO_S_MESSAGE = 1,
        S_TO_C_MESSAGE = 2
    };

    enum PacketErrorCode : uint32_t {
        PACKET_ERROR_CODE_NONE,
    };
}