using Bb.ComponentModel.Loaders;
using System.Reflection.PortableExecutable;

namespace Bb.Servers.Web
{


    /// <summary>
    /// ServiceRunner
    /// </summary>
    public class ServiceRunner<TStartup>
        : ServiceRunnerBase, IDisposable
        where TStartup : ServiceRunnerStartup
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRunner{TStartup}"/> class.
        /// </summary>
        /// <param name="start">if set to <c>true</c> [start].</param>
        /// <param name="args">The arguments.</param>
        public ServiceRunner(params string[] args)
            : base(args)
        {

        }
                
        protected override void TuneHostBuilder(IWebHostBuilder webBuilder)
        {           
            webBuilder.UseStartup<TStartup>();
        }

        protected override void TuneWebHostBuilder(WebApplicationBuilder webBuilder)
        {
            _startup = (TStartup)Activator.CreateInstance(typeof(TStartup), new object[] { webBuilder.Configuration });
            _startup.ConfigureServices(webBuilder.Services);
        }

        protected override void ConfigureApplication(WebApplication wbuilder, IWebHostEnvironment environment, ILoggerFactory? loggerFactory)
        {
            base.ConfigureApplication(wbuilder, environment, loggerFactory);
            if (_startup != null)
                _startup.ConfigureApplication(wbuilder, environment, loggerFactory!);


        }

        private TStartup? _startup;

    }

}
