#include <iostream>
#include <boost/asio.hpp>
#include <boost/date_time/posix_time/posix_time.hpp>

using boost::asio::ip::tcp;
class session : public std::enable_shared_from_this<session>
{
public :
    session(tcp::socket socket) : socket_(std::move(socket)) {
    }

    void start() {
        do_read();
    }

private:
    tcp::socket socket_;
    enum { max_length = 1024 };
    char data_[max_length];

    void do_read() {
        auto self(shared_from_this());
        socket_.async_read_some(boost::asio::buffer(data_, max_length),
            [this, self](boost::system::error_code ec, std::size_t length)
            {
                if (!ec)
                {
                    do_write(length);
                }
                else {
                    // 그 외의 오류
                    std::cout << "Error occurred: " << ec.message() << std::endl;
                    if (socket_.is_open())
                    {
                        std::cout << "Socket is Close"  << std::endl;
                    }
                }
            });
    }

    void do_write(std::size_t length)
    {
        auto self(shared_from_this());
        boost::asio::async_write(socket_, boost::asio::buffer(data_, length),
            [this, self](boost::system::error_code ec, std::size_t /*length*/)
            {
                if (!ec)
                {
                    do_read();
                }
            });
    }
};


class server
{
public:
    server(boost::asio::io_context& io_context, short port)
        : acceptor_(io_context, tcp::endpoint(tcp::v4(), port)), socket_(io_context)
    {
        std::cout << "Server is running on: "
            << "IP: " << tcp::endpoint(tcp::v4(), port).address().to_string()
            << ", Port: " << port << std::endl;
        
        do_accept();  // 클라이언트 연결 대기 시작
    }

private:
    void do_accept()
    {
        acceptor_.async_accept(socket_,
            [this](boost::system::error_code ec)
            {
                if (!ec)
                {
                    std::cout << "Client connected!ip [ " << socket_.remote_endpoint().address().to_string() << " ]" << std::endl;
                    std::make_shared<session>(std::move(socket_))->start();
                }
                else
                {
                    std::cout << ec.what() << std::endl;
                }
               
                do_accept();
            });
    }

    tcp::endpoint endpoint_;
    tcp::acceptor acceptor_;
    tcp::socket socket_;
  
};


int main(int argc, char* argv[])
{
    auto port = "7777";

    try
    {
        boost::asio::io_context io_context;
        server s(io_context, std::atoi(port));

        io_context.run();
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}


