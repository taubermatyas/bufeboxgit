using Newtonsoft.Json;
using Swashbuckle.Application;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BufeBackEnd
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // ✅ CORS engedélyezése frontendre (React fejlesztői szerverről)
            var cors = new EnableCorsAttribute("http://localhost:3000", "*", "*");
            config.EnableCors(cors);

            // ✅ Csak JSON válaszok
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            // ✅ Hivatkozási hurkok kiküszöbölése
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            // ✅ Attribútum alapú routing
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Swagger redirect route (opcionális)
            config.Routes.MapHttpRoute(
                name: "swagger",
                routeTemplate: "",
                defaults: null,
                constraints: null,
                handler: new RedirectHandler((url => url.RequestUri.ToString()), "swagger")
            );
        }
    }
}
