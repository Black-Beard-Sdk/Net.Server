# Net.Server
Provide a service base for just concentrate on services

[![Build status](https://ci.appveyor.com/api/projects/status/5723pgaqrbys7g74?svg=true)](https://ci.appveyor.com/project/gaelgael5/net-server)


## Installation
```bash
$ install-package Black.Beard.Web.Server
```


## Usage
```csharp

    /// <summary>
    /// Startup class par parameter
    /// </summary>
    public class Startup : StartupBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration) 
            : base(configuration)
        {
      
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Yours configuration           
            base.ConfigureServices(services);
        }
    
        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public override void ConfigureApplication(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {

            base.ConfigureApplication(app, env, loggerFactory);
                        
            app
                .UseHttpsRedirection()
                .UseRouting()
                .UseListener()
                ;

        }

    }

```

```csharp

    internal class Program
    {

        /// <summary>
        /// Main entry point of the application.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var service = GetService(args);
            service.Run();
        }

        public static ServiceRunnerBase GetService(string[] args)
        {              
            return new ServiceRunner<Startup>(args);
        }

    }

```


You can configure WebApplicationBuilder like that
```csharp


    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplicationBuilder : ApplicationInitializerBase<WebApplicationBuilder>
    {
               
        public override void Execute(WebApplicationBuilder builder)
        {

            var services = builder.Services;

            services.AddAuthorization(options =>
            {

                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });

            });


        }

    }

```


You can configure service like that
```csharp

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
    public class ModuleInitializeWebApplication : ApplicationInitializerBase<WebApplication>
    {
              
        public override void Execute(WebApplication builder)
        {
            
        }

    }
```

All json files existing in the root of the application and in the folder Configs are loaded, and you can append configuration fragment like that
```csharp

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IInjectBuilder<IConfigurationBuilder>))]
    public class ConfigurationBuilderBuilder : InjectBuilder<IConfigurationBuilder>
    {

        public override object Execute(IConfigurationBuilder service)
        {

            service.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("Configs/appsettings.json", optional: true, reloadOnChange: true)
                ;

            return 0;
        }
    }

```


You can inject configuration like that. in the sqample ConnectionStringSettings is the name of the section in the configuration documents
```csharp

    [ExposeClass(ConstantsCore.Configuration, ConfigurationKey = "ConnectionStringSettings", LifeCycle = IocScopeEnum.Transiant)]
    public partial class ConnectionSettings
    {

        public ConnectionSettings()
        {
            this.ConnectionStringSettings = new ConnectionStringSettings();
        }

        public ConnectionStringSettings ConnectionStringSettings { get; set; }

    }

    // You can ask the configuration like that
    var connectionSettings = builder.Services.GetService<Option<ConnectionSettings>>();

```



You can inject service like that
```csharp

    [ExposeClass(ConstantsCore.Service, LifeCycle = IocScopeEnum.Transiant)]
    public partial class MyService : IService
    {
        public MyService()
        {
        }

        public void Execute()
        {
            Console.WriteLine("MyService");
        }
    }

    // You can ask the service like that
    var instance = builder.Services.GetService<MyService>();

```



You can using the injection like that
```csharp

    TService uiService = builder.Services.GetService<TService>(); 
    var loader = new InjectionLoader<TService>(UIConstants.LeftMenu, builder.Services)
        .LoadModules(c =>
        {

        })
    .Execute(uiService);

```



# WebAssembly


```ps
    Install-Package Black.Beard.WebAssembly
```


In the program.cs you can use the following code
```csharp


    WebApplicationBuilder builder = WebApplication.CreateBuilder(args).Initialize(ConstantsCore.Initialization,
    x =>
    {
        
    });


    WebApplication app = builder.Build().Initialize(ConstantsCore.Initialization, 
    x =>
    {
        
    });


    app.Run();

```


```csharp


    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplicationBuilder>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializer : ApplicationInitializerBase<WebApplicationBuilder>
    {

        public override void Execute(WebApplicationBuilder builder)
        {
            

        }

    }

    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
    public class WebApplicationBuilderInitializer : ApplicationInitializerBase<WebApplication>
    {

        public override void Execute(WebApplication app)
        {
            

        }

    }

```
