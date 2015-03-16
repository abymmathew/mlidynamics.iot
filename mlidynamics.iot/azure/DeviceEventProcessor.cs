using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using mlidynamics.iot.models;
using Microsoft.ServiceBus.Messaging;

namespace mlidynamics.iot.azure
{
    public class DeviceEventProcessor : IEventProcessor
    {
        public static readonly DataContractSerializer TelemetryModelSerializer =
            new DataContractSerializer(typeof (TelemetryModel));

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            return Task.FromResult<object>(null);
        }

        public Task OpenAsync(PartitionContext context)
        {
            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var message in messages)
            {
            }

            return Task.FromResult<object>(null);
        }
    }
}