using App.Core.Infrastructure;
using App.Web.Admin.Factories;
using App.Web.Admin.Factories.Logging;
using App.Web.Admin.Factories.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Web.Framework.Infrastructure
{
    public partial class NopStartup : INopStartup
    {
        public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //factories
            services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
            services.AddScoped<IScheduleTaskModelFactory, ScheduleTaskModelFactory>();
            services.AddScoped<IActivityLogTypeModelFactory, ActivityLogTypeModelFactory>();
            services.AddScoped<ILogModelFactory, LogModelFactory>();
            services.AddScoped<IQueuedEmailModelFactory, QueuedEmailModelFactory>();
        }

        public void Configure(IApplicationBuilder application)
        {
        }

        public int Order => 2900;
    }
}