using System.Threading.Tasks;
using System.Web.Http;
using mlidynamics.iot.models;

namespace mlidynamics.iot.io.Controllers
{
    [RoutePrefix("api/Device")]
    public class DeviceController : ApiController
    {
        [Route("Publish")]
        public async Task<IHttpActionResult> Publish(TelemetryModel message)
        {
            // do processing here

            return Ok();
        }

        public async Task<IHttpActionResult> Register(DeviceInfoModel message)
        {
            // do processing here

            return Ok();
        }
    }
}