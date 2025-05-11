//
// blocking_tcp_echo_client.cpp
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//
// Copyright (c) 2003-2025 Christopher M. Kohlhoff (chris at kohlhoff dot com)
//z
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
//

#include <cstdlib>
#include <cstring>
#include <iostream>
#include "NetworkManager.h"

int main(int argc, char* argv[])
{
    try
    { 
        std::string exitCommand = "bye";

       auto& networkManager =NetworkManager::GetInstance();
       networkManager.SetConnect("localhost", "7777");

        for (;;)
        {
            std::cout << "Enter message: ";

            std::string input;
            std::getline(std::cin, input);
            
            auto packet = PacketFactory::CreatePacket<Packet::C2SMessage>();
            packet->SetMessage(input);
            
            networkManager.SendPacket(std::static_pointer_cast<Packet::IPacket>(packet));

            std::cout << "Reply is: ";
            networkManager.ReceivePacket();
           

        }
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}
