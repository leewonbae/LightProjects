#include <cstdint>
#include <vector>
#include <string>

#include "Commons.h"
#include "json.hpp"

using json = nlohmann::json;
namespace Packet {
	class BasePacket {
	public:
		commons::Protocols GetProtocol() const {
			return _protocol;
		}
		void SetProtocol(commons::Protocols protocol) {
			_protocol = protocol;
		}

		void SetPacketSize(json body)
		{
			_packetSize = sizeof(commons::Protocols) + sizeof(commons::PacketErrorCode);
			_packetSize+= body.dump().size();
		}

		uint32_t GetPacketSize() const {
			return _packetSize;
		}

		std::string GetBody() {
			return _body;
		}
	private:
		commons::Protocols _protocol = commons::Protocols::PROTOCOL_NONE;
		commons::PacketErrorCode _packetErrorCode = commons::PacketErrorCode::PACKET_ERROR_CODE_NONE;
		std::uint32_t _packetSize;
		json _body;
	};


	class IPacket {
	public:
		IPacket(commons::Protocols protocol)
		{
			_protocol = protocol;
		}
		virtual ~IPacket() = default;
		commons::Protocols GetProtocol() const { return _protocol; }

		// 직렬화/역직렬화 인터페이스
		virtual json ToJson() const = 0;
		virtual void FromJson(const json& j) = 0;

	protected:
		commons::Protocols _protocol;
	};

	class ReqMessage : public IPacket {
	public:
		ReqMessage() :IPacket(commons::Protocols::C_TO_S_MESSAGE) {}
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
namespace PacketFactory {
	template<typename T>
	static std::unique_ptr<T> CreatePacket() {
		return std::make_unique<T>();
	}
}