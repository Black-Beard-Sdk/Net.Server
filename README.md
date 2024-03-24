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
        /// Configures the custom services.
        /// </summary>
        /// <param name="services"></param>
        public override void AppendServices(IServiceCollection services)
        {
            RegisterServicesPolicies(services);
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

                //.UseApiKey()                      // Intercept apiKey and create identityPrincipal associated
                //.UseAuthorization()               // Apply authorization for identityPrincipal
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