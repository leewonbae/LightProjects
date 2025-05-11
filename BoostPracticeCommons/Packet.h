#pragma once
#include <cstdint>
#include <vector>
#include <string>

#include "Commons.h"
#include "Json.h"

using json = nlohmann::json;

namespace PacketFactory {
	template<typename T>
	static std::shared_ptr<T> CreatePacket() {
		return std::make_shared<T>();
	}
}

namespace Packet {
	class IPacket {
	public:
		IPacket(commons::Protocols protocol)
		{
			_protocol = protocol;
		}
		virtual ~IPacket() = default;
		commons::Protocols GetProtocol() const { return _protocol; }
		commons::PacketErrorCode GetPacketErrorCode() const { return _packetErrorCode; };
		
		// 직렬화/역직렬화 인터페이스
		virtual json ToJson() const = 0;
		virtual void FromJson(const json& j) = 0;

	protected:
		commons::Protocols _protocol;
		commons::PacketErrorCode _packetErrorCode = commons::PacketErrorCode::PACKET_ERROR_CODE_NONE;
	};

	class C2SMessage : public IPacket {
	public:
		C2SMessage() :IPacket(commons::Protocols::C_TO_S_MESSAGE) {}
		void SetMessage(std::string& message)
		{
			_message = message;
		}

		json ToJson() const override {
			json j;
			j["protocol"] = static_cast<uint32_t>(GetProtocol());
			j["message"] = _message;
			return j;
		}

		void FromJson(const json& j) override
		{
			_protocol = static_cast<commons::Protocols>(j.at("protocol").get<uint32_t>());
			_message = j.at("message").get<std::string>();
		}

	private:
		std::string _message;
	};
}
