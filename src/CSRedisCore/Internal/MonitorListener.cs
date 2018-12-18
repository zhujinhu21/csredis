using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CSRedis.Internal
{
    class MonitorListener : RedisListner<object>
    {
        public event EventHandler<RedisMonitorEventArgs> MonitorReceived;

        public MonitorListener(RedisConnector connection, ILogger logger = null)
            : base(connection, logger)
        { }

        public string Start()
        {
            string status = Call(RedisCommands.Monitor());
            Listen(x => x.Read());
            return status;
        }

        protected override void OnParsed(object value)
        {
            OnMonitorReceived(value);
            if (_logger != null) _logger.LogTrace("Message received.", new object[] { value });
        }

        protected override bool Continue()
        {
            return Connection.IsConnected;
        }

        void OnMonitorReceived(object message)
        {
            if (MonitorReceived != null)
                MonitorReceived(this, new RedisMonitorEventArgs(message));
        }
    }
}
