using System;
using System.Collections.Generic;

namespace RabbitMQAdpater
{
    public class Config
    {
        public static bool CheckEnvsExist(params string[] keys)
        {
            Dictionary<string, string> env = GetEnvironmentVariables();
            bool result = true;
            string errorMsg = "";

            List<string> envNames = new List<string>(keys);
            envNames.ForEach(name =>
            {
                if (!env.ContainsKey(name))
                {
                    result = env.ContainsKey(name);
                    errorMsg += " " + name;
                }
            });
            if (!result)
            {
                string keysString = "";
                for (int i = 0; i < keys.Length; i++)
                {
                    keysString += " " + keys[i];
                }
                Log.Color($"Miss EnvironmentVariables [{errorMsg}]. We need {keysString}]", ConsoleColor.Red);
            }
            return result;
        }
        /// <summary>
        /// 將 System.Collections.IDictionary 轉成 System.Collections.Generic.Dictionary
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetEnvironmentVariables()
        {
            //Dictionary<string, string> result = new Dictionary<string, string>()
            //{
            //    {"MQUserName","guest" },
            //    {"MQExchangeName","Config" },
            //    {"MQPassword","guest" },
            //    {"MQHostName","192.168.2.5" },
            //    {"MQVirualHost","/" },
            //    {"MQPort","5672" },
            //    {"RedisChannelname","MyChannel" },
            //    {"MQFilenameKeyInHeader","ConfigName" },
            //};
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (System.Collections.DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                if (!result.ContainsKey(de.Key.ToString()))
                    result.Add(de.Key.ToString(), de.Value.ToString());
            }
            return result;
        }
    }
}
