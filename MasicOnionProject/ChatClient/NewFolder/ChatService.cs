using Grpc.Net.Client;
using MagicOnion.Client;
using Snowpipe.Commons.ServicePackets;
using Snowpipe.Commons.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.NewFolder
{
    public class ChatReceiver : IChattingSteamingHubReceiver
    {
        private IChattingSteamingHub? _client;

        public async Task<bool> OnConnected(string serverUrl)
        {
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:5002");

            try
            {
                _client = await StreamingHubClient.ConnectAsync<IChattingSteamingHub, IChattingSteamingHubReceiver>(channel, this);
            }
            catch (Exception ex)
            {
                return false;
            }
            

            return true;
        }

        public void OnChatMessage(ScChatMessagePacket packet)
        {
            Console.WriteLine($"Received chat message: {packet.Message}");
        }

        public void OnRoomIn(ScRoomInPacket packet)
        {
            Console.WriteLine($"User {packet.AccountIdList} entered the room.");
        }

        public void OnRoomOut(ScRoomOutPacket packet)
        {
            Console.WriteLine($"User {packet.AccountId} left the room.");
        }
    }
}
