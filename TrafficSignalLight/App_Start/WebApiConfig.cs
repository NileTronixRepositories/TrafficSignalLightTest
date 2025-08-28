using System.Web.Http;
using System.Web.Http.Cors;

namespace TrafficSignalLight
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            var cors = new EnableCorsAttribute(
                   "http://127.0.0.1:5500,http://localhost:5500",
                   "*",
                   "*")
            {
                SupportsCredentials = true
            };
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}