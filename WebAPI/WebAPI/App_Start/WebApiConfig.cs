using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;
using WebAPI.Filters;

namespace WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            //config.Filters.Add(new UserAuthorizationFilterAttribute());
            config.Filters.Add(new UnauthorizedExceptionAttribute());
            config.Filters.Add(new NotFoundExceptionAttribute());
            config.Filters.Add(new ForbiddenExceptionAttribute());
            config.Filters.Add(new BadRequestExceptionAttribute());
            config.Filters.Add(new ConflictExceptionAttribute());

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterType<UserRepository>().AsSelf().SingleInstance();

            var container = builder.Build();

            var repo = container.Resolve<UserRepository>();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            config.EnsureInitialized();
        }
    }
}
