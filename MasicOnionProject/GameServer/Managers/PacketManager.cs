using BattleServer.Attributes;
using BattleServer.Exceptions;
using BattleServer.Extentions;
using GameServer.Helpers;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Snowpipe.Commons.BattleServerCommons;
using Snowpipe.Commons.BattleServerCommons.BattlePackets;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;
using static Google.Apis.Requests.RequestError;

namespace BattleServer.Managers
{
    [BattleInjectableClass(ServiceLifetime.Singleton)]
    public class PacketManager
    {
        private ConcurrentDictionary<Guid, Channel<BaseCsPacket>> _receiverToQueue;
        private ILogger<PacketManager> _logger;
        private BattleManager _battleManager;
        public PacketManager(ILogger<PacketManager> logger, BattleManager battleManager)
        {
            _logger = logger;
            _battleManager = battleManager;

            _receiverToQueue = new ConcurrentDictionary<Guid, Channel<BaseCsPacket>>();
        }

        // connect되었을 때 hub 에서 호출 
        public void AddReceiverQueue(Guid contextId, IBattleStreamingHubReceiver receiver)
        {
            var newChannel = Channel.CreateUnbounded<BaseCsPacket>(new UnboundedChannelOptions()
            {
                SingleReader = false,
                SingleWriter = true,
            });

            // 찌꺼기 세션 방어
            if (_receiverToQueue.TryRemove(contextId, out var oldChannel))
            {
                oldChannel.Writer.TryComplete();
            }

            _receiverToQueue.TryAdd(contextId, newChannel);

            Task.Run(() => ProcessPacketConsumer(contextId, receiver, newChannel));
        }

        // disconnect되었을 때 hub 에서 호출 
        public void RemoveReciverQueue(Guid contextId)
        {
            // 💡 1. 딕셔너리에서 먼저 안전하게 제거하여 새로운 패킷이 인큐되는 것을 막습니다.
            if (_receiverToQueue.TryRemove(contextId, out var channel))
            {
                // 💡 2. 채널의 Writer를 닫습니다. 
                // 이 호출 즉시 ProcessPacketConsumer 내부의 대기 루프가 자연스럽게 종료(Exit)됩니다.
                channel.Writer.TryComplete();
            }
        }

        public void EnqueuePacket(Guid contextId, BaseCsPacket baseCsPacket)
        {
            if (_receiverToQueue.TryGetValue(contextId, out var channel))
            {
                try
                {
                    if (channel.Writer.TryWrite(baseCsPacket))
                    {
                        // 큐에 적재 성공
                        _logger.LogInformation($"ContextId [{contextId}] Accumulated Queue Count : {channel.Reader.Count}");
                    }
                    else
                    {
                        // 큐에 적재 실패
                        _logger.LogWarning($"Failed to write packet to channel for context: {contextId}");
                    }
                }
                catch (ChannelClosedException)
                {
                    _logger.LogWarning($"ChannelClosedException {contextId}");
                }
            }
            else
            {
                // disConnect로 없어진 경우
                _logger.LogWarning($"Already Disconnected Context [{contextId}]");
            }
        }

        private async Task ProcessPacketConsumer(Guid contextId, IBattleStreamingHubReceiver receiver, Channel<BaseCsPacket> channel)
        {
            try
            {
                await foreach (var baseCsPacket in channel.Reader.ReadAllAsync())
                {
                    _logger.LogInformation("Received packet from client. ProtocolType: " + baseCsPacket.ProtocolType);

                    var latency = (ServerDateTime.Now - baseCsPacket.CreateDt).TotalMilliseconds;
                    _logger.LogInformation($"[Req] [{baseCsPacket.ProtocolType.ToString()}] Packet latency: {latency} ms");

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    try
                    {
                        switch (baseCsPacket.ProtocolType)
                        {
                            case E_PROTOCOL_TYPE.CS_BATTLE_MATCH_REQUEST:
                                _battleManager.BattleMatchRequest(receiver, baseCsPacket);
                                break;
                        }
                    }
                    catch (BattleServerException bsex)
                    {
                        receiver.SendToClient(new BaseScPacket()
                        {
                            ErrorCode = bsex.PacketErrorCode
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"[Error] : " + ex.Message);

                        receiver.SendToClient(new BaseScPacket()
                        {
                            ErrorCode = Snowpipe.Commons.Packets.E_PACKET_ERROR_CODE.SERVER_ERROR
                        });

                    }
                    finally
                    {
                        stopwatch.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                // 💡 백그라운드 태스크 루프 자체가 알 수 없는 이유로 터졌을 때의 최종 방어선
                _logger.LogCritical($"[Fatal] Packet consumer loop crashed for ContextId [{contextId}]. Exception: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation($"[Consumer Closed] Loop finished safely for ContextId [{contextId}].");
            }
        }
    }
}
