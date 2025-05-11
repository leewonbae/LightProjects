#include "NetworkManager.h"
#include <boost/asio.hpp>
#include <iostream>

void NetworkManager::SetConnect(const std::string ip,const std::string & port)
{
    _socket = std::make_unique<tcp::socket>(_io_context);
    tcp::resolver resolver(_io_context);

    boost::asio::connect(*_socket, resolver.resolve(ip, port));
}

void NetworkManager::ReceivePacket()
{
    char header[12];
    size_t header_length = boost::asio::read(*_socket, boost::asio::buffer(header, 12));

    uint32_t resProtocol;
    uint32_t resPacketErrorCode;
    uint32_t resJsonSize;

    memcpy(&resProtocol, header, sizeof(uint32_t));
    memcpy(&resPacketErrorCode, header + sizeof(uint32_t), sizeof(uint32_t));
    memcpy(&resJsonSize, header + sizeof(uint32_t) + sizeof(uint32_t), sizeof(uint32_t));

    std::cout << "resProtocol -> " << resProtocol << "resPacketErrorCode ->" << resPacketErrorCode << "resJsonSize ->" << resJsonSize << std::endl;

    char body[max_length];
    size_t body_length = boost::asio::read(*_socket, boost::asio::buffer(body, resJsonSize));
    std::string resJson(body, resJsonSize);
    std::cout << "resJson " << resJson << std::endl;
}

void NetworkManager::SendPacket(std::shared_ptr<Packet::IPacket> packet)
{
    size_t offset = 0;
    auto packetJson = packet->ToJson();
    char request[max_length];
    // protocol
    uint32_t protocol = static_cast<uint32_t>(packet->GetProtocol());
    memcpy(request + offset, &protocol, sizeof(uint32_t));
    offset += sizeof(commons::Protocols);

    // packetErrorCode
    uint32_t packetErrorCode = static_cast<uint32_t>(packet->GetPacketErrorCode());
    memcpy(request + offset, &packetErrorCode, sizeof(uint32_t));
    offset += sizeof(commons::PacketErrorCode);

    // bodySize
    std::string jsonStr = packetJson.dump();
    uint32_t jsonSize = static_cast<uint32_t>(jsonStr.size());
    std::cout << "jsonSize = " << jsonSize << std::endl;
    memcpy(request + offset, &jsonSize, sizeof(uint32_t)); // JSON 길이도 4바이트로 저장
    offset += sizeof(uint32_t);

    // body
    memcpy(request + offset, jsonStr.data(), jsonStr.size());
    offset += jsonStr.size();

    std::cout << "TotalPacketSize = " << offset << std::endl;
    boost::asio::write(*_socket, boost::asio::buffer(request, offset));

}