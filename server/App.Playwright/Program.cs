using App.Automation.Hubs;
using App.Automation.Services;
using App.Playwright.Infrastructure.Extensions;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel server options
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    // You can also set other limits here as needed
});

//load application settings
builder.Services.ConfigureApplicationSettings(builder);

var useAutofac = true;

if (useAutofac)
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
else
    builder.Host.UseDefaultServiceProvider(options =>
    {
        //we don't validate the scopes, since at the app start and the initial configuration we need 
        //to resolve some services (registered as "scoped") through the root container
        options.ValidateScopes = false;
        options.ValidateOnBuild = true;
    });

//add services to the application and configure service provider
builder.Services.ConfigureApplicationServices(builder);

// Add services to the container.
builder.Services.AddAppMvc();
builder.Services.AddCors(options => // Add this for CORS
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyOrigin()
        //.WithOrigins(
        //    "https://office.serefidis.gr", "https://officedebug.serefidis.local", "https://office.serefidis.local",
        //    "https://localhost:7191", "http://localhost:5141")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});
builder.Services.AddSignalR();
builder.Services.AddScoped<ILocalizationService, LocalizationService>();

var app = builder.Build();

//configure the application HTTP request pipeline
app.ConfigureRequestPipeline();

app.UseHttpsRedirection();

app.UseCors("CorsPolicy"); // Use the CORS policy

app.UseAuthorization();

app.MapControllers();

app.MapHub<ProgressHub>("/hubs/progress"); // Map the SignalR hub

app.Run();
