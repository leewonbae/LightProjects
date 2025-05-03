#include <cstdint>
#include <vector>
#include <string>

#include "Commons.h"
#include "json.hpp"

using json = nlohmann::json;

class BasePacket
{
public:
    BasePacket(){}
    virtual ~BasePacket() {}
    
    void SetProtocol(commons::Protocols protocol)
    {
        _protocol = protocol;
    }
    commons::Protocols GetProtocol() const
    {
        return _protocol;
    }

    void SetPacketErrorCode(commons::PacketErrorCode packetErrorCode)
    {
        _packetErrorCode = packetErrorCode;
    }

    commons::PacketErrorCode GetPacketErrorCode() const
    {
        return _packetErrorCode;
    }
    
    void SetPacket(const std::string& serializedPacket)
    {
        _packet = std::vector<char>(serializedPacket.begin(), serializedPacket.end());
        _basePacketSize =  sizeof(_protocol) + sizeof(_packetErrorCode) + sizeof(_basePacketSize)+ serializedPacket.length();
    }
    
    uint32_t GetBasePacketSize() const{
        return _basePacketSize;
    }

private:
    commons::Protocols _protocol = commons::Protocols::NONE;
    commons::PacketErrorCode _packetErrorCode = commons::PacketErrorCode::NONE;
    uint32_t _basePacketSize = 0;
    std::vector<char> _packet;
};