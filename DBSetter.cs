using System;
using System.Threading;
using System.Net.Sockets;
using CSRedis;

namespace RabbitMQAdpater
{
    public class DBSetter
    {
        public static RedisClient RedisConnect(string ip)
        {
            RedisClient result = null;
            TimeSpan oneSec = TimeSpan.FromSeconds(1);

            while (true)
            {
                result = new RedisClient(ip)
                {
                    ReconnectAttempts = 3,
                    ReconnectWait = 200
                };
                try
                {
                    if (result.Connect((int)oneSec.TotalMilliseconds))
                        break;
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == 10061)
                    {
                        Log.Color(LogTable.Msg5, ConsoleColor.Red);
                    }
                }
                Thread.Sleep((int)oneSec.TotalMilliseconds);
            }
            Console.WriteLine(LogTable.Msg6);
            return result;
        }
    }
}
