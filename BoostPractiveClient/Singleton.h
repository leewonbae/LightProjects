#pragma once

template <typename T>
class Singleton {
public:
	static T& GetInstance()
	{
		static T instance;
		return instance;
	}
	
	Singleton(const Singleton&) = delete; // 복사 생성자 사용 안함
	Singleton& operator=(const Singleton&) = delete; // 대입 연산자 사용 안함

protected:
	Singleton() = default;
	virtual ~Singleton() = default;

};