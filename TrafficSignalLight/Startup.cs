using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System.Threading.Tasks;
using System.Web.Cors;

[assembly: OwinStartup(typeof(TrafficSignalLight.Startup))]

namespace TrafficSignalLight
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
            var policy = new CorsPolicy
            {
                AllowAnyHeader = true,
                AllowAnyMethod = true,
                SupportsCredentials = true,
                AllowAnyOrigin = true,
            };
            policy.Origins.Add("http://127.0.0.1:5500");
            policy.Origins.Add("http://localhost:5500");
            policy.Origins.Add("http://localhost:58196");
            policy.Origins.Add("http://localhost:4200");

            var corsOptions = new CorsOptions
            {
                PolicyProvider = new CorsPolicyProvider
                {
                    PolicyResolver = context => Task.FromResult(policy)
                }
            };

            var hubConfig = new HubConfiguration { EnableDetailedErrors = true };

            // ✅ CORS لِـ SignalR فقط
            app.Map("/signalr", map =>
            {
                map.UseCors(corsOptions);
                map.RunSignalR(hubConfig);
            });
            //  app.MapSignalR();
        }
    }
}