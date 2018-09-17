using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Harry.Extensions.Redis
{
    public static class RedisManager
    {
        private static ConnectionMultiplexer _instance = null;
        private static object locker = new object();

        public static string Configuration { get; set; } = "127.0.0.1";

        /// <summary>
        /// 单例模式获取redis连接实例
        /// </summary>
        public static ConnectionMultiplexer Instance
        {
            get
            {
                if (_instance != null && _instance.IsConnected)
                    return _instance;

                lock (locker)
                {
                    if (_instance != null && _instance.IsConnected)
                        return _instance;

                    if (_instance != null)
                    {
                        try
                        {
                            _instance.Dispose();
                        }
                        catch { }
                    }

                    _instance = ConnectionMultiplexer.Connect(Configuration);
                    return _instance;
                }
            }
        }


    }

}
