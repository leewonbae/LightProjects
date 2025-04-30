#include <iostream>
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
    // 메모리 누수 감지를 활성화합니다.
    _CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);

    std::cout << "Hello World!"<<std::endl;

    int* intArray = new int[100];
    int* testArray = new int[100];

    auto testClassPtr = std::make_shared<testClass>();
    testClassPtr->AddCount();

    TestFunc(testClassPtr);
    testClassPtr->Print();


    return 0;
}


