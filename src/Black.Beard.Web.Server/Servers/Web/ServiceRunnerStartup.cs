﻿using Bb.ComponentModel;
using Bb.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Bb.Servers.Web.Swashbuckle;
using Bb.Servers.Web.Models;
using Bb.Servers.Web.Models.Security;
using Bb.Servers.Web.Middlewares.EntryFullLogger;
using Bb.Servers.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bb.Servers.Web
{

    public class ServiceRunnerStartup
    {


        public ServiceRunnerStartup(IConfiguration configuration)
        {
            AssemblyInformations = Assembly.GetEntryAssembly().GetAssemblyInformation();
            CurrentConfiguration = configuration;
            Configuration = CurrentConfiguration.Get<GlobalConfiguration>();
        }


        #region Configure


        public virtual void ConfigureServices(IServiceCollection services)
        {

            // Auto discover all types with attribute [ExposeClass] for register in ioc.
            RegisterTypes(services);

            // see : https://learn.microsoft.com/fr-fr/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-7.0#fhmo
            services.Configure<ForwardedHeadersOptions>(ConfigureForwardedHeadersOptions);

            if (Configuration.UseSwagger) // Swagger OpenAPI 
                RegisterServicesSwagger(services);

            if (Configuration.UseTelemetry)
                RegisterTelemetry(services);         

            if (Configuration.UseControllers)
                services.AddControllers(ConfigureControllers);

        }



        protected virtual void ConfigureForwardedHeadersOptions(ForwardedHeadersOptions options)
        {

            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor
              | ForwardedHeaders.XForwardedProto;

            //options.ForwardLimit = 2;
            //options.KnownProxies.Add(IPAddress.Parse("127.0.10.1"));
            //options.ForwardedForHeaderName = "X-Forwarded-For-My-Custom-Header-Name";

        }

        /// <summary>
        /// Configure the controllers.
        /// </summary>
        /// <param name="options"></param>
        protected virtual void ConfigureControllers(MvcOptions options)
        {

        }

        protected virtual void ConfigureInterceptExceptions(IApplicationBuilder c)
        {
            c.Run(InterceptExceptions);
        }

        protected async Task InterceptExceptions(HttpContext context)
        {

            var response = new HttpExceptionModel
            {
                Origin = AssemblyInformations?.AssemblyTitle ?? "web services",
                TraceIdentifier = context.TraceIdentifier,
                Session = context.Session.Id
            };


            if (GlobalConfiguration.IsDevelopment)
            {
                var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
                if (exceptionHandler != null)
                {
                    var error = exceptionHandler.Error;
                    response.Message = error.ToString();
                }
            }


            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(response);
        }

        protected virtual void ConfigureEnvironmentProduction(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders();
        }

        protected virtual void ConfigureEnvironmentDevelopment(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHsts();
            app.UseDeveloperExceptionPage();
        }

        protected virtual void ConfigureSwagger(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger(c => { })
               .UseSwaggerUI(c => { })
            ;
        }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            if (Configuration.UseSwagger) ConfigureSwagger(app, env, loggerFactory);
            if (Configuration.UseTelemetry) ConfigureTelemetry(app, env, loggerFactory);
            if (Configuration.TraceAll) app.UseMiddleware<RequestResponseLoggerMiddleware>();

            if (!env.IsDevelopment()) ConfigureEnvironmentDevelopment(app, env, loggerFactory);
            else ConfigureEnvironmentProduction(app, env, loggerFactory);

            ConfigureApplication(app, env, loggerFactory);

            if (Configuration.UseControllers)
                app.UseEndpoints(MapController);

        }


        protected virtual void MapController(IEndpointRouteBuilder endpoints)
        {

            endpoints.MapControllers();

        }


        public virtual void ConfigureApplication(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            app.UseExceptionHandler(ConfigureInterceptExceptions);

            //if (Configuration.TraceAll)
            //    app.UseHttpInfoLogger();             // log entries requests

        }

        #endregion Configure

        protected virtual void RegisterServicesSwagger(IServiceCollection services)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-5.0&tabs=visual-studio
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {

                c.DescribeAllParametersInCamelCase();
                c.IgnoreObsoleteActions();
                c.AddDocumentation(i =>
                {
                    i.Licence(l => l.Name("Only usable with a valid partner contract."));
                }, DocumentationFilter);
                c.AddSwaggerWithApiKeySecurity(services, CurrentConfiguration);
                c.DocumentFilter<AppendInheritanceDocumentFilter>();
                c.OperationFilter<FileResultContentTypeOperationFilter>();
                c.UseOneOfForPolymorphism();

            });
        }

        /// <summary>
        /// Filter for tacking only documentation file specified by filter.
        /// </summary>
        public string DocumentationFilter { get; set; } = "Black.*.xml";

        public void RegisterTelemetry(IServiceCollection services)
        {

            var resource = ResourceBuilder.CreateDefault().AddService("Black.Beard.Web.Server");

            var builder = services.AddOpenTelemetry();

            builder.WithTracing((builder) =>
            {

                var activities = Diagnostics.DiagnosticProviderExtensions.GetActivityNames();
                var keys = activities.Select(c => c.Item2).ToArray();

                builder.SetResourceBuilder(resource)
                    .AddConsoleExporter()
                    .AddAspNetCoreInstrumentation(o =>
                    {

                        o.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("requestProtocol", httpRequest.Protocol);
                        };
                        o.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("responseLength", httpResponse.ContentLength);
                        };
                        o.EnrichWithException = (activity, exception) =>
                         {
                             activity.SetTag("exceptionType", exception.GetType().ToString());
                         };

                    })
                    .AddSource(keys)
                    ;
            });

            builder.WithMetrics((builder) =>
            {

                var meters = Diagnostics.DiagnosticProviderExtensions.GetMeterNames();
                var keys = meters.Select(c => c.Item2).ToArray();

                builder.SetResourceBuilder(resource)
                    .AddConsoleExporter()
                    //.AddAspNetCoreInstrumentation()
                    .AddMeter(keys);

            });

        }

        public virtual void ConfigureTelemetry(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {


        }

        /// <summary>
        /// Auto discover all types with attribute [ExposeClass] for register  in ioc.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public virtual void RegisterTypes(IServiceCollection services)
        {

            // Auto discover all types with attribute [ExposeClass] for register in ioc.
            services.UseTypeExposedByAttribute(CurrentConfiguration, ConstantsCore.Configuration, c =>
            {
                services.BindConfiguration(c, CurrentConfiguration);
            });

            services.UseTypeExposedByAttribute(CurrentConfiguration, ConstantsCore.Model)
                    .UseTypeExposedByAttribute(CurrentConfiguration, ConstantsCore.Service);

        }

        /// <summary>
        /// evaluate permissions.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="policy">The policy.</param>
        /// <returns></returns>
        protected async Task<bool> Authorize(AuthorizationHandlerContext arg, PolicyModel policy)
        {

            if (arg.User != null)
            {

                var res = (DefaultHttpContext)arg.Resource;
                var path = res.Request.Path;

                PolicyModelRoute route = policy.Evaluate(path);

                var i = arg.User.Identity as ClaimsIdentity;
                var roles = i.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();

                //if (roles.Where(c => c.Value == ""))

            }

            await Task.Yield();

            return true;

        }

        protected AssemblyInformations AssemblyInformations { get; }

        public IConfiguration CurrentConfiguration { get; }

        public GlobalConfiguration? Configuration { get; }





        //protected virtual void RegisterServicesPolicies(IServiceCollection services)
        //{

        //    // Auto discovers all services with Authorize attribute and 
        //    // Initialize security policies for apply permissions based on identityPrincipal authorizations
        //    var policies = PoliciesExtension.GetReferencedPolicies();
        //    if (policies.Any())
        //        services.AddAuthorization(options =>
        //        {
        //            foreach (var policyModel in policies)
        //                options.AddPolicy(policyModel.Name, policy => policy.RequireAssertion(a => Authorize(a, policyModel)));

        //        });
        //    var currentAssembly = Assembly.GetAssembly(GetType());
        //    policies.SaveInFolder(Path.GetDirectoryName(currentAssembly.Location));
        //}


    }



}
