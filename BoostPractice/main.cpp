#include <iostream>
#include <boost/asio.hpp>
#include <boost/date_time/posix_time/posix_time.hpp>
class testClass
{
public:
    testClass() {
        _currentCount = 0;
        std::cout << "생성자" << std::endl;
    }
    ~testClass() {
        std::cout << "소멸자" << std::endl;
    }
    void AddCount() {
        _currentCount += 1;
    }

    void Print() {
        std::cout << _currentCount << std::endl;
    }

private:
    int32_t _currentCount;
    

};
void TestFunc(std::shared_ptr<testClass> testClassPtr) {
    testClassPtr->AddCount();
}

int main()
{
    
    boost::asio::io_context io;
    boost::asio::deadline_timer t(io, boost::posix_time::seconds(5));
    t.wait();
   
    std::cout << "Hello World!"<<std::endl;

    system("pause");
    return 0;
}


