using App.Core.Configuration;
using App.Core.Domain.Common;
using App.Core.Http;
using App.Core.Infrastructure;
using App.Core.Security;
using App.Data;
using App.Services.Common;
using App.Services.Helpers;
using App.Services.Jwt;
using App.Services.Security;
using App.Validators;
using App.Web.Framework.Mvc.ModelBinding;
using App.Web.Framework.Mvc.ModelBinding.Binders;
using App.Web.Framework.Mvc.Routing;
using App.Web.Framework.Security.Captcha;
using App.Web.Framework.WebOptimizer;
using Azure.Identity;
using Azure.Storage.Blobs;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Converters;
using StackExchange.Profiling.Storage;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebMarkupMin.AspNetCore7;
using WebMarkupMin.Core;
using WebMarkupMin.NUglify;

namespace App.Web.Framework.Infrastructure.Extensions
{
    /// <summary>
    /// Represents extensions of IServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure base application settings
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationSettings(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //let the operating system decide what TLS protocol version to use
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

            //create default file provider
            CommonHelper.DefaultFileProvider = new NopFileProvider(builder.Environment);

            //register type finder
            var typeFinder = new WebAppTypeFinder();
            Singleton<ITypeFinder>.Instance = typeFinder;
            services.AddSingleton<ITypeFinder>(typeFinder);

            //add configuration parameters
            var configurations = typeFinder
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();

            foreach (var config in configurations)
            {
                //builder.Configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);
                var section = builder.Configuration.GetSection(config.Name);

                if (config is BearerTokenConfig)
                {
                    // Correctly setup options and validation for BearerTokenConfig
                    services.AddOptions<BearerTokenConfig>()
                            .Bind(section)
                            .Validate(options =>
                            {
                                return options.AccessTokenExpirationMinutes < options.RefreshTokenExpirationMinutes;
                            }, "Access token expiration must be less than refresh token expiration.");
                }
                else
                {
                    // Bind other configurations normally
                    section.Bind(config, options => options.BindNonPublicProperties = true);
                }
            }

            var appSettings = AppSettingsHelper.SaveAppSettings(configurations, CommonHelper.DefaultFileProvider, false);
            services.AddSingleton(appSettings);
        }

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureApplicationServices(this IServiceCollection services,
            WebApplicationBuilder builder)
        {
            //add accessor to HttpContext
            services.AddHttpContextAccessor();

            //initialize plugins
            //var mvcCoreBuilder = services.AddMvcCore();
            services.AddMvc(options => { options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()); });
            //create engine and configure service provider
            var engine = EngineContext.Create();

            engine.ConfigureServices(services, builder.Configuration);
        }

        /// <summary>
        /// Register HttpContextAccessor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        /// <summary>
        /// Adds services required for anti-forgery support
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddAntiForgery(this IServiceCollection services)
        {
            //override cookie name
            services.AddAntiforgery(options =>
            {
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.AntiforgeryCookie}";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// Adds services required for application session state
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddHttpSession(this IServiceCollection services)
        {
            services.AddSession(options =>
            {
                options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.SessionCookie}";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
        }

        /// <summary>
        /// Adds services required for distributed cache
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddDistributedCache(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            var distributedCacheConfig = appSettings.Get<DistributedCacheConfig>();

            if (!distributedCacheConfig.Enabled)
                return;

            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;

                case DistributedCacheType.SqlServer:
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = distributedCacheConfig.ConnectionString;
                        options.SchemaName = distributedCacheConfig.SchemaName;
                        options.TableName = distributedCacheConfig.TableName;
                    });
                    break;

                case DistributedCacheType.Redis:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                    });
                    break;
            }
        }

        /// <summary>
        /// Adds data protection services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopDataProtection(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<AzureBlobConfig>().Enabled && appSettings.Get<AzureBlobConfig>().StoreDataProtectionKeys)
            {
                var blobServiceClient = new BlobServiceClient(appSettings.Get<AzureBlobConfig>().ConnectionString);
                var blobContainerClient = blobServiceClient.GetBlobContainerClient(appSettings.Get<AzureBlobConfig>().DataProtectionKeysContainerName);
                var blobClient = blobContainerClient.GetBlobClient(NopDataProtectionDefaults.AzureDataProtectionKeyFile);

                var dataProtectionBuilder = services.AddDataProtection().PersistKeysToAzureBlobStorage(blobClient);

                if (!appSettings.Get<AzureBlobConfig>().DataProtectionKeysEncryptWithVault)
                    return;

                var keyIdentifier = appSettings.Get<AzureBlobConfig>().DataProtectionKeysVaultId;
                var credentialOptions = new DefaultAzureCredentialOptions();
                var tokenCredential = new DefaultAzureCredential(credentialOptions);

                dataProtectionBuilder.ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), tokenCredential);
            }
            else
            {
                var dataProtectionKeysPath = CommonHelper.DefaultFileProvider.MapPath(NopDataProtectionDefaults.DataProtectionKeysPath);
                var dataProtectionKeysFolder = new System.IO.DirectoryInfo(dataProtectionKeysPath);

                //configure the data protection system to persist keys to the specified directory
                services.AddDataProtection().PersistKeysToFileSystem(dataProtectionKeysFolder);
            }
        }

        /// <summary>
        /// Adds authentication service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopAuthentication(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            var config = appSettings.Get<BearerTokenConfig>();

            // Needed for jwt auth.
            services
                .AddAuthentication(options =>
                {
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = config.Issuer, // site that makes the token
                        ValidAudience = config.Audience, // site that consumes the token
                        ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
                        ValidateAudience = false, // TODO: change this to avoid forwarding attacks
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Key)),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    };
                    cfg.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            //var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
                            //return logger.ErrorAsync("Authentication failed.", context.Exception);
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
                            return tokenValidatorService.ValidateAsync(context);
                        },
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                if (path.StartsWithSegments("/hubs"))
                                {
                                    // Read the token out of the query string
                                    context.Token = accessToken;
                                }
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            //var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
                            //return logger.ErrorAsync("OnChallenge error", context.AuthenticateFailure);
                            Console.WriteLine("OnChallenge error", context.AuthenticateFailure);
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        /// <summary>
        /// Add and configure MVC for the application
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <returns>A builder for configuring MVC services</returns>
        public static IMvcBuilder AddNopMvc(this IServiceCollection services)
        {
            //add basic MVC feature
            var mvcBuilder = services.AddControllers();
            //var mvcBuilder = services.AddControllersWithViews();

            mvcBuilder.AddRazorRuntimeCompilation();

            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<CommonConfig>().UseSessionStateTempDataProvider)
            {
                //use session-based temp data provider
                mvcBuilder.AddSessionStateTempDataProvider();
            }
            else
            {
                //use cookie-based temp data provider
                mvcBuilder.AddCookieTempDataProvider(options =>
                {
                    options.Cookie.Name = $"{NopCookieDefaults.Prefix}{NopCookieDefaults.TempDataCookie}";
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                });
            }

            services.AddRazorPages();

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            //mvcBuilder.AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            //MVC now serializes JSON with camel case names by default, use this code to avoid it
            mvcBuilder.AddNewtonsoftJson(options => {

                //options.SerializerSettings.Converters.Add(new Newtonsoft.Json.TimeSpanConverter());
                var dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.DateTimeConverter(dateTimeHelper));
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.DateTimeNullableConverter(dateTimeHelper));
                options.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;

                // Hack is for the issue described in this post:
                // http://stackoverflow.com/questions/11789114/internet-explorer-json-net-javascript-date-and-milliseconds-issue
                options.SerializerSettings.Converters.Add(new IsoDateTimeConverter
                {
                    DateTimeFormat = "yyyy-MM-dd\\THH:mm:ss.fffK"
                    // DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"
                });
                // Needed because JSON.NET does not natively support I8601 Duration formats for TimeSpan
                //options.SerializerSettings.Converters.Add(new TimeSpanConverter());
                //options.SerializerSettings.Converters.Add(new StringEnumConverter());

                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                //options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

                if (options.SerializerSettings.ContractResolver is Newtonsoft.Json.Serialization.DefaultContractResolver resolver)
                {
                    //resolver.NamingStrategy = null;  // remove json camelCasing; names are converted on the client.
                    resolver.NamingStrategy.ProcessDictionaryKeys = true;
                }
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented; // format JSON for debugging                
            });

            //set some options
            mvcBuilder.AddMvcOptions(options =>
            {
                //we'll use this until https://github.com/dotnet/aspnetcore/issues/6566 is solved 
                options.ModelBinderProviders.Insert(0, new InvariantNumberModelBinderProvider());
                options.ModelBinderProviders.Insert(1, new CustomPropertiesModelBinderProvider());
                //add custom display metadata provider 
                options.ModelMetadataDetailsProviders.Add(new NopMetadataProvider());

                //in .NET model binding for a non-nullable property may fail with an error message "The value '' is invalid"
                //here we set the locale name as the message, we'll replace it with the actual one later when not-null validation failed
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(_ => NopValidationDefaults.NotNullValidationLocaleName);
            });

            //add fluent validation
            services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();

            //register all available validators from Nop assemblies
            var assemblies = mvcBuilder.PartManager.ApplicationParts
                .OfType<AssemblyPart>()
                .Where(part => part.Name.StartsWith("App", StringComparison.InvariantCultureIgnoreCase))
                .Select(part => part.Assembly);
            services.AddValidatorsFromAssemblies(assemblies);

            //register controllers as services, it'll allow to override them
            mvcBuilder.AddControllersAsServices();

            return mvcBuilder;
        }

        /// <summary>
        /// Register custom RedirectResultExecutor
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopRedirectResultExecutor(this IServiceCollection services)
        {
            //we use custom redirect executor as a workaround to allow using non-ASCII characters in redirect URLs
            services.AddScoped<IActionResultExecutor<RedirectResult>, NopRedirectResultExecutor>();
        }

        /// <summary>
        /// Add and configure MiniProfiler service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopMiniProfiler(this IServiceCollection services)
        {
            //whether database is already installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            var appSettings = Singleton<AppSettings>.Instance;
            if (appSettings.Get<CommonConfig>().MiniProfilerEnabled)
            {
                services.AddMiniProfiler(miniProfilerOptions =>
                {
                    //use memory cache provider for storing each result
                    ((MemoryCacheStorage)miniProfilerOptions.Storage).CacheDuration = TimeSpan.FromMinutes(appSettings.Get<CacheConfig>().DefaultCacheTime);

                    //determine who can access the MiniProfiler results
                    miniProfilerOptions.ResultsAuthorize = request => EngineContext.Current.Resolve<IPermissionService>().AuthorizeAsync(StandardPermissionProvider.AccessProfiling).Result;
                });
            }
        }

        /// <summary>
        /// Add and configure WebMarkupMin service
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopWebMarkupMin(this IServiceCollection services)
        {
            //check whether database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            services
                .AddWebMarkupMin(options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                    options.DisableMinification = !EngineContext.Current.Resolve<CommonSettings>().EnableHtmlMinification;
                    options.DisableCompression = true;
                    options.DisablePoweredByHttpHeaders = true;
                })
                .AddHtmlMinification(options =>
                {
                    options.MinificationSettings.AttributeQuotesRemovalMode = HtmlAttributeQuotesRemovalMode.KeepQuotes;

                    options.CssMinifierFactory = new NUglifyCssMinifierFactory();
                    options.JsMinifierFactory = new NUglifyJsMinifierFactory();
                })
                .AddXmlMinification(options =>
                {
                    var settings = options.MinificationSettings;
                    settings.RenderEmptyTagsWithSpace = true;
                    settings.CollapseTagsWithoutContent = true;
                });
        }

        /// <summary>
        /// Adds WebOptimizer to the specified <see cref="IServiceCollection"/> and enables CSS and JavaScript minification.
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopWebOptimizer(this IServiceCollection services)
        {
            var appSettings = Singleton<AppSettings>.Instance;
            var cssBundling = appSettings.Get<WebOptimizerConfig>().EnableCssBundling;
            var jsBundling = appSettings.Get<WebOptimizerConfig>().EnableJavaScriptBundling;

            //add minification & bundling
            var cssSettings = new CssBundlingSettings
            {
                FingerprintUrls = false,
                Minify = cssBundling
            };

            var codeSettings = new CodeBundlingSettings
            {
                Minify = jsBundling,
                AdjustRelativePaths = false //disable this feature because it breaks function names that have "Url(" at the end
            };

            services.AddWebOptimizer(null, cssSettings, codeSettings);
        }

        /// <summary>
        /// Add and configure default HTTP clients
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public static void AddNopHttpClients(this IServiceCollection services)
        {
            //default client
            services.AddHttpClient(NopHttpDefaults.DefaultHttpClient).WithProxy();

            //client to request current store
            services.AddHttpClient<StoreHttpClient>();

            //client to request current store
            services.AddHttpClient<OllamaHttpClient>();

            //client to request appCommerce official site
            services.AddHttpClient<NopHttpClient>().WithProxy();

            //client to request appCommerce official site
            services.AddHttpClient<PlaywrightHttpClient>().ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler
                {
                    // WARNING: Only use in development environments; insecure for production
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
                };
            });

            ////client to request appCommerce official site
            //services.AddHttpClient<PlaywrightHttpClient>().ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            //{
            //    ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            //    {
            //        // Custom validation logic
            //        if (sslPolicyErrors == SslPolicyErrors.None)
            //        {
            //            return true; // Certificate is valid
            //        }

            //        // Add custom logic to validate certificate
            //        // Example: Check if the certificate thumbprint matches a known good value
            //        var knownGoodCertThumbprint = "YOUR_KNOWN_GOOD_CERT_THUMBPRINT";
            //        if (cert.GetCertHashString() == knownGoodCertThumbprint)
            //        {
            //            return true; // Allow specific certificate
            //        }

            //        Console.WriteLine($"SSL certificate error: {sslPolicyErrors}");
            //        return false; // Certificate is invalid
            //    }
            //});

            //client to request reCAPTCHA service
            services.AddHttpClient<CaptchaHttpClient>().WithProxy();
        }
    }
}