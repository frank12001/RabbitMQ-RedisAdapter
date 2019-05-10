using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RabbitMQAdpater
{
    public class K8sTcpHeathCheck
    {
        readonly Func<bool> Readiness;
        readonly Func<bool> Liveness;
        readonly int ReadinessPort;
        readonly int LivelessPort;

        TcpListener Readlistener;
        TcpListener Livelistener;
        
        public K8sTcpHeathCheck(int readinessPort, int livenessPort, Func<bool> readinessCallback, Func<bool> livenessCallback)
        {
            Readiness = readinessCallback;
            Liveness = livenessCallback;

            ReadinessPort = readinessPort;
            LivelessPort = livenessPort;

            TcpListener GetListener(int port)
            {
                TcpListener result = new TcpListener(IPAddress.IPv6Any, port);
                result.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
                result.Start();
                return result;
            }

            Task.Run(async ()=> 
            {
                while (true)
                {
                    if (Readlistener == null && Readiness())
                    {
                        Readlistener = GetListener(ReadinessPort);
                    }
                    else
                    {
                        if (Liveness() && (Livelistener == null))
                        {
                            Livelistener = GetListener(LivelessPort);
                        }
                        if(!Liveness() && (Livelistener != null))
                        {
                            Livelistener?.Stop();
                        }
                    }
                    await Task.Delay((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
                }
            });
        }

    }
}
