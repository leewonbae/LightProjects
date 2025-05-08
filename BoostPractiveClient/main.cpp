//
// blocking_tcp_echo_client.cpp
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
// Copyright (c) 2003-2025 Christopher M. Kohlhoff (chris at kohlhoff dot com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
//

#include <cstdlib>
#include <cstring>
#include <iostream>
#include <boost/asio.hpp>
#include "../BoostPracticeCommons/Packet.h"

using boost::asio::ip::tcp;

enum { max_length = 1024 };

int main(int argc, char* argv[])
{
    try
    {
        auto port = "7777";
        std::string exitCommand = "bye";

        boost::asio::io_context io_context;

        tcp::socket socket(io_context);
        tcp::resolver resolver(io_context);
        boost::asio::connect(socket, resolver.resolve("localhost", port));

        for (;;)
        {
            std::cout << "Enter message: ";
            char request[max_length];
            size_t offset = 0;
            std::string input;
            std::getline(std::cin, input);
            //
            auto packet = PacketFactory::CreatePacket<Packet::C2SMessage>();
            packet->SetMessage(input);
            auto packetJson = packet->ToJson();

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
            memcpy(request+ offset, jsonStr.data(), jsonStr.size());
            offset += jsonStr.size();

            std::cout << "TotalPacketSize = " << offset << std::endl;
            boost::asio::write(socket, boost::asio::buffer(request, offset));

            std::cout << "Reply is: ";
            char header[12];
            size_t header_length = boost::asio::read(socket, boost::asio::buffer(header, 12));

            uint32_t resProtocol;
            uint32_t resPacketErrorCode;
            uint32_t resJsonSize;

            memcpy(&resProtocol, header, sizeof(uint32_t));
            memcpy(&resPacketErrorCode, header+ sizeof(uint32_t), sizeof(uint32_t));
            memcpy(&resJsonSize, header + sizeof(uint32_t) + sizeof(uint32_t), sizeof(uint32_t));
            
            std::cout << "resProtocol -> " << resProtocol << "resPacketErrorCode ->" << resPacketErrorCode << "resJsonSize ->" << resJsonSize << std::endl;
            
            char body[max_length];
            size_t body_length = boost::asio::read(socket, boost::asio::buffer(body, resJsonSize));
            std::string resJson(body, resJsonSize);
            std::cout << "resJson " << resJson << std::endl;
           /* std::cout.write(reply, reply_length);
            std::cout << "\n";

            std::string reply_string(reply, reply_length);
            if (exitCommand == reply_string)
            {
                break;
            }*/

        }
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}
