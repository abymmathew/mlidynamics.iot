using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using mlidynamics.iot.models;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

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

        public async Task ProcessEventsAsync(PartitionContext partitionContext, IEnumerable<EventData> messages)
        {
            try
            {
                foreach (var message in messages)
                {
                    var data = Encoding.UTF8.GetString(message.GetBytes());

                    // do something interesting here
                    var telemetryModel = JsonConvert.DeserializeObject<TelemetryModel>(data);

                    Trace.TraceInformation("Message received.  Partition: '{0}', Device: '{1}', Data: '{2}'",
                        partitionContext.Lease.PartitionId, telemetryModel.DeviceId, telemetryModel.Temperature);
                }
            }
            catch (Exception exp)
            {
                Trace.TraceError("Error in processing: " + exp.Message);
            }

            if (_checkpointStopWatch.Elapsed.Ticks > TimeSpan.FromMinutes(5).Ticks)
            {
                await partitionContext.CheckpointAsync();
                lock (this)
                {
                    _checkpointStopWatch.Restart();
                }
            }
        }
    }
}