#include "Singleton.h"
#include <boost/asio.hpp>
#include <memory>  // std::unique_ptr 사용
#include "../BoostPracticeCommons/Packet.h"

using boost::asio::ip::tcp;

enum { max_length = 1024 };
class NetworkManager : public Singleton<NetworkManager>
{
    friend class Singleton<NetworkManager>;  // Singleton<Logger>는 생성자에 접근 가능
public:
    void SetConnect(const std::string ip, const std::string& port);

    void ReceivePacket();

    void SendPacket(std::shared_ptr<Packet::IPacket> packet);

private:
    NetworkManager() = default;
    boost::asio::io_context _io_context;
    std::unique_ptr<tcp::socket> _socket;  // 소켓을 unique_ptr로 관리
};

