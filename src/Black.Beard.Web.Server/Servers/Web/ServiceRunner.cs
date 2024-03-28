using Bb.ComponentModel;
using Bb.Extensions;
using Bb.Helpers;

namespace Bb.Servers.Web
{


    /// <summary>
    /// ServiceRunner
    /// </summary>
    public class ServiceRunner<TStartup>
        : ServiceRunnerBase, IDisposable
        where TStartup : class
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


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="startPorts">The starting range port</param>
        /// <param name="count">The count range port</param>
        /// <returns></returns>
        public ServiceRunner<TStartup> AddLocalhostUrl(string scheme, ref int startPorts, int count = 1)
        {

            int _first = startPorts;

            List<string> ports = new List<string>();
            for (int i = 0; i < count; i++)
                AddLocalhostUrl(scheme, _first = HttpHelper.GetAvailablePort(_first) + 1);

            startPorts = _first;

            return this;

        }

        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="hostName">The host name to listen</param>
        /// <param name="port">The port to listen.</param>
        /// <returns></returns>
        public ServiceRunner<TStartup> AddUrl(string scheme, string hostName, int port)
        {

            if (_urls == null)
                _urls = new List<Uri>();

            _urls.Add(scheme.ToUri(hostName, port));

            return this;
        }

        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="port">The port to listen</param>
        /// <returns></returns>
        public ServiceRunner<TStartup> AddLocalhostUrl(string scheme, int port)
        {
            AddUrl(scheme, "localhost", port);
            return this;
        }

        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="host">The host name. if null localhost is used by default</param>
        /// <param name="startingPort">starting port to search.</param>
        /// <returns></returns>
        public ServiceRunner<TStartup> AddLocalhostSecureUrlWithDynamicPort(string? host, ref int startingPort)
        {
            return AddLocalhostUrlWithDynamicPort("https", host, ref startingPort);
        }

        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="host">The host name. if null localhost is used by default</param>
        /// <param name="startingPort">starting port to search.</param>
        /// <returns></returns>
        public ServiceRunner<TStartup> AddLocalhostUrlWithDynamicPort(string? host, ref int startingPort)
        {
            return AddLocalhostUrlWithDynamicPort("http", host, ref startingPort);
        }

        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="host">The host name. if null localhost is used by default</param>
        /// <param name="startingPort">starting port to search.</param>
        /// <returns></returns>
        public ServiceRunner<TStartup> AddLocalhostUrlWithDynamicPort(string scheme, string? host, ref int startingPort)
        {

            if (_urls == null)
                _urls = new List<Uri>();

            var ports = _urls.Select(c => c.Port).OrderBy(c => c).ToList();

            startingPort = HttpHelper.GetAvailablePort(startingPort);

            while (ports.Contains(startingPort))
            {
                startingPort++;
                startingPort = HttpHelper.GetAvailablePort(startingPort);
            }

            AddUrl(scheme, host ?? "localhost", startingPort);

            return this;
        }


        //public virtual ServiceRunner<TStartup> StartService()
        //{
        //    RunAsync();
        //    return this;
        //}


        protected override void TuneHostBuilder(IWebHostBuilder webBuilder)
        {
            webBuilder.UseStartup<TStartup>();
        }
              
    }

}
