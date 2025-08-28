using Microsoft.AspNet.SignalR;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TrafficSignalLight.Dto;

namespace TrafficSignalLight.Controllers
{
    //[Route("api/signal")]
    public class SignalController : ApiController
    {
        [HttpPost]
        [Route("api/signal/log")]
        public async Task Log(TrafficLightResponse response)
        {
            response.IpAdress = HttpContext.Current.Request.UserHostAddress;
            string message = JsonSerializer.Serialize(response);
            var hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            hub.Clients.All.ReceiveMessage("Server", message ?? "");
            //hub.Clients.All.TrafficSignal(message);
            var testMessage = "http://197.168.208.50/7B0100000A0005000D00000000000000007D";

            return;
        }
    }
}