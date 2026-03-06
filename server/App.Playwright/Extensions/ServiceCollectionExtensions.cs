using App.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System.Net;

namespace App.Playwright.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureApplicationSettings(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //let the operating system decide what TLS protocol version to use
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            //create default file provider
            CommonHelper.DefaultFileProvider = new NopFileProvider(builder.Environment);

            //register type finder
            var typeFinder = new WebAppTypeFinder();
            Singleton<ITypeFinder>.Instance = typeFinder;
            services.AddSingleton<ITypeFinder>(typeFinder);
        }

        public static void ConfigureApplicationServices(this IServiceCollection services, WebApplicationBuilder builder)
        {
            //add accessor to HttpContext
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //initialize plugins
            services.AddMvcCore();
            //create engine and configure service provider
            var engine = EngineContext.Create();

            engine.ConfigureServices(services, builder.Configuration);
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddAppMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddControllers();
            //var mvcBuilder = services.AddControllersWithViews();

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddNewtonsoftJson(options => {

                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss.fffK"
                });

                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                if (options.SerializerSettings.ContractResolver is Newtonsoft.Json.Serialization.DefaultContractResolver resolver)
                {
                    //resolver.NamingStrategy = null;  // remove json camelCasing; names are converted on the client.
                    resolver.NamingStrategy.ProcessDictionaryKeys = true;
                }
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // format JSON for debugging                
            });

            return mvcBuilder;
        }

    }
}