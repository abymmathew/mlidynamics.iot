using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using mlidynamics.iot.azure;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WorkerRole1
{
    public class EventHubProcessorWorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private EventProcessorHost _eventProcessorHost;

        public override void Run()
        {
            Trace.TraceInformation("WorkerRole1 is running");

            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var result = base.OnStart();

            Trace.TraceInformation("WorkerRole1 has been started");

            var azureStorageConnectionString = CloudConfigurationManager.GetSetting("Azure.Storage.ConnectionString");
            var microsoftServiceBusConnectionString =
                CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var eventHubName = CloudConfigurationManager.GetSetting("eventHubName");
            var eventProcessorHostName = Guid.NewGuid().ToString();

            var client = EventHubClient.CreateFromConnectionString(microsoftServiceBusConnectionString, eventHubName);

            Trace.TraceInformation("Consumer group is: " + client.GetDefaultConsumerGroup().GroupName);

            _eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName,
                client.GetDefaultConsumerGroup().GroupName, microsoftServiceBusConnectionString,
                azureStorageConnectionString);

            Trace.TraceInformation("Created event processor host...");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("WorkerRole1 is stopping");

            _eventProcessorHost.UnregisterEventProcessorAsync().Wait();

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("WorkerRole1 has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            await _eventProcessorHost.RegisterEventProcessorAsync<DeviceEventProcessor>();

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}