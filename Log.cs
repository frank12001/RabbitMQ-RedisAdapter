using System;

namespace RabbitMQAdpater
{
    public class Log
    {        
        public static void Color(string msg, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(msg);
            Console.ResetColor();
        }
    }

    public class LogTable
    {
        public const string Msg1 = "Server Start.";
        public const string Msg2 = "Receive from RabbotMQ.But don't have header or don't have header named ";
        public const string Msg3 = "Connect to RabbitMQ Success.";
        public const string Msg4 = "header is not string.";
        public const string Msg5 = "Cann't Connect to Redis.";
        public const string Msg6 = "Connect to Redis Success.";
        public const string Msg7 = "Miss EnvironmentVariables";
    }
}
