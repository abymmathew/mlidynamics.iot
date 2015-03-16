using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.ServiceBus.Messaging;

namespace mlidynamics.iot.azure
{
    public class DeviceEventHubReceiver
    {
        private EventProcessorHost eventProcessorHost;
        
        readonly EventHubClient _client =
            EventHubClient
            .CreateFromConnectionString(ConfigurationManager.AppSettings[@"Microsoft.ServiceBus.ConnectionString"], "mlidynamics-iot-telemetry");

        public void RegisterEventProcessor()
        {
            string eventProcessorHostName = Guid.NewGuid().ToString();

            eventProcessorHost = new EventProcessorHost(eventProcessorHostName,);
        }

        public void UnRegisterEventProcessor()
        {
        }
    }
}
