using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using mlidynamics.iot.models;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace mlidynamics.iot.io.Controllers
{
    [RoutePrefix("api/Device")]
    public class DeviceController : ApiController
    {
        private readonly EventHubClient _client =
            EventHubClient
                .CreateFromConnectionString(ConfigurationManager.AppSettings[@"Microsoft.ServiceBus.ConnectionString"],
                    "mlidynamics-iot-telemetry");

        [Route("Publish")]
        public async Task<IHttpActionResult> Publish(TelemetryModel message)
        {
            // do processing here
            var jsonString = JsonConvert.SerializeObject(message);

            _client.Send(new EventData(Encoding.UTF8.GetBytes(jsonString))
            {
                PartitionKey = "0"
            });

            return Ok();
        }

        [Route("Register")]
        public async Task<IHttpActionResult> Register(DeviceInfoModel message)
        {
            // do processing here

            return Ok();
        }
    }
}