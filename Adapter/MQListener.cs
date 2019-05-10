using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace RabbitMQAdpater
{
    public class MQListener
    {
        /// <summary>
        /// 建立 RabbitMQ 需要的環境變數的 Key Name
        /// </summary>
        private static readonly string[] EnvNames = new string[] { "MQUserName", "MQPassword", "MQVirualHost", "MQHostName", "MQPort", "MQExchangeName" };
        public static MQConnectObj RabbitMQConnect(MQSetting setting, Action<MQCallback> MQCallback)
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                UserName = setting.UserName,
                Password = setting.Password,
                HostName = setting.HostName,
                Port = setting.Port,
                VirtualHost = setting.VirualHost
            };

            IConnection connection = null;
            while (true)
            {
                try
                {
                    connection = factory.CreateConnection();
                    break;
                }
                catch (BrokerUnreachableException e)
                {
                    Log.Color(e.Message,ConsoleColor.Red);
                }
                Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
            }
            Console.WriteLine(LogTable.Msg3);

            var channel = connection.CreateModel();

            channel.ExchangeDeclare(setting.ExchangeName, "fanout");
            var queueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(queue: queueName, exchange: setting.ExchangeName, routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Dictionary<string, string> header = new Dictionary<string, string>();
                if (ea.BasicProperties.Headers != null)
                {
                    foreach (KeyValuePair<string, object> raw in ea.BasicProperties.Headers)
                    {
                        try
                        {
                            string s = Encoding.UTF8.GetString((byte[])raw.Value);
                            header.Add(raw.Key, s);
                        }
                        catch (InvalidCastException e)
                        {
                            Console.WriteLine(LogTable.Msg4);
                            continue;
                        }
                    }
                }
                MQCallback?.Invoke(new MQCallback() { Header = header, Msg = Encoding.UTF8.GetString(ea.Body) });
            };
            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            return new MQConnectObj() { Factory = factory, Connection = connection, Channel = channel };
        }

        public static MQSetting TryCreateByEnvs()
        {
            Dictionary<string, string> env = Config.GetEnvironmentVariables();

            bool result = Config.CheckEnvsExist(EnvNames);
            if (!result)
                throw new Exception(LogTable.Msg7);
            List<string> envNames = new List<string>(EnvNames);
            return new MQSetting()
            {
                UserName = env[envNames[0]],
                Password = env[envNames[1]],
                VirualHost = env[envNames[2]],
                HostName = env[envNames[3]],
                Port = int.Parse(env[envNames[4]]),
                ExchangeName = env[envNames[5]]
            };
        }
    }

    public struct MQSetting
    {
        public string UserName;
        public string Password;
        public string HostName;
        public int Port;
        public string VirualHost;
        public string ExchangeName;
    }
    public struct MQCallback
    {
        public Dictionary<string, string> Header;
        public string Msg;
    }
    public struct MQConnectObj : IDisposable
    {
        public ConnectionFactory Factory;
        public IConnection Connection;
        public IModel Channel;

        public void Dispose()
        {
            Connection.Dispose();
            Channel.Dispose();
        }
    }
}
