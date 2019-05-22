using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CSRedis;

namespace RabbitMQAdpater
{
    class Program
    {
        private readonly static string[] Keys = new string[]
            {"RedisChannelname", "MQFilenameKeyInHeader", "ReadinessPort", "LivenessPort"};

        private static K8sTcpHeathCheck heathChecker;

        static void Main(string[] args)
        {
            if (!Config.CheckEnvsExist(Keys))
            {
                Log.Color(LogTable.Msg7, ConsoleColor.Red);
                return;
            }

            //將需要的參數從環境變數中取出
            Dictionary<string, string> envs = Config.GetEnvironmentVariables();
            string RedisIP = "localhost";
            string RedisChannelname = envs[Keys[0]];
            string MQFilenameKeyInHeader = envs[Keys[1]];
            int ReadinessPort = int.Parse(envs[Keys[2]]);
            int LivenessPort = int.Parse(envs[Keys[3]]);

            //建立 Redis (一個連線) , RabbitMQ (一個連線) , K8sHeathCheck (2 個 tcp port)
            using (RedisClient redis = DBSetter.RedisConnect(RedisIP))
            {
                MQSetting mqSetting = MQListener.TryCreateByEnvs();

                void RabbitCallback(MQCallback mq)
                {
                    if (mq.Header != null && mq.Header.TryGetValue(MQFilenameKeyInHeader, out string header))
                    {
                        Console.WriteLine("header ConfigName : " + header);
                        Console.WriteLine(" Msg " + mq.Msg);

                        redis.Set(header, mq.Msg);
                        redis.Publish(RedisChannelname, header);
                    }
                    else
                        Console.WriteLine($"{LogTable.Msg2} {MQFilenameKeyInHeader}");
                }

                using (MQConnectObj mqConnection = MQListener.RabbitMQConnect(mqSetting, RabbitCallback))
                {
                    Console.WriteLine(LogTable.Msg1);

                    bool LivenessCheck()
                    {
                        try
                        {
                            redis.Ping();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return false;
                        }

                        return (redis.IsConnected && mqConnection.Connection.IsOpen);
                    }

                    heathChecker = new K8sTcpHeathCheck(ReadinessPort, LivenessPort,
                        () => { return (redis.IsConnected && mqConnection.Connection.IsOpen); },
                        LivenessCheck);
                    while (true)
                    {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
    }
}
