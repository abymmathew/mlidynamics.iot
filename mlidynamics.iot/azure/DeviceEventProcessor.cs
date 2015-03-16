using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace mlidynamics.iot.azure
{
    public class DeviceEventProcessor : IEventProcessor
    {
        private Stopwatch _checkpointStopWatch;

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        public Task OpenAsync(PartitionContext context)
        {
            lock (this)
            {
                _checkpointStopWatch = new Stopwatch();
                _checkpointStopWatch.Start();
            }

            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var message in messages)
            {
                var data = Encoding.UTF8.GetString(message.GetBytes());

                // do something interesting here
            }

            if (_checkpointStopWatch.Elapsed.Ticks > TimeSpan.FromMinutes(5).Ticks)
            {
                await context.CheckpointAsync();
                lock (this)
                {
                    _checkpointStopWatch.Restart();
                }
            }
        }
    }
}