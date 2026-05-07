using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace GameServer.Utils;

public static class NetworkUtil
{
    //public static string IPAddress
    //{
    //    get
    //    {
    //        var host = Dns.GetHostEntry(Dns.GetHostName());

    //        foreach (var ip in host.AddressList)
    //        {
    //            if (ip.AddressFamily == AddressFamily.InterNetwork)
    //            {
    //                return ip.ToString();
    //            }
    //        }

    //        return "127.0.0.1";
    //    }
    //}

    public static string IPAddress
    {
        get
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in interfaces)
            {
                // VMware 가 설치된경우 해당 아이피는 제외
                if (adapter.Description.Contains("Virtual")
                    || adapter.Description.Contains("Loopback")
                    || adapter.Description.Contains("lo"))
                {
                    continue;
                }

                var ipCollections = adapter.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation ip in ipCollections)
                {
                    // IPV4 아이피인경우만 가져옴
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.Address.ToString();
                    }
                }
            }

            return "127.0.0.1";
        }
    }
}

