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

        tcp::socket s(io_context);
        tcp::resolver resolver(io_context);
        boost::asio::connect(s, resolver.resolve("localhost", port));

        for (;;)
        {
            std::cout << "Enter message: ";
            char request[max_length];
            std::string input;
            std::getline(std::cin, input);
            size_t request_length = std::strlen(request);

            //
            auto packet = PacketFactory::CreatePacket<Packet::ReqMessage>();
            packet->SetMessage(input);
            
            auto basePacket = std::make_shared<Packet::BasePacket>();
            basePacket->SetProtocol(packet->GetProtocol());
            basePacket->SetPacketSize(packet->ToJson());
            
            //
            boost::asio::write(s, boost::asio::buffer(input, input.length()));

            char reply[max_length];
            size_t reply_length = boost::asio::read(s, boost::asio::buffer(reply, request_length));
            std::cout << "Reply is: ";
            std::cout.write(reply, reply_length);
            std::cout << "\n";

            std::string reply_string(reply, reply_length);
            if (exitCommand == reply_string)
            {
                break;
            }

        }
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}
