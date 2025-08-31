using Newtonsoft.Json;
using System.Net.Http.Headers;
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
                   "*",
                   "*",
                   "*")
            {
                SupportsCredentials = true
            };
            config.EnableCors(cors);
            config.Formatters.JsonFormatter.SupportedMediaTypes
      .Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // (اختياري) منع loop
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}