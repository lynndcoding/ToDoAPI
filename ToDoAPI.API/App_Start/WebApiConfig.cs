using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ToDoAPI.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //Below we add the following line of code to create the behavior in our application that will allow cross origin resource sharing. This allows all controllers to add this functionality, if we add the metadata to the top of its class (see ResourcesController).
            config.EnableCors();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
