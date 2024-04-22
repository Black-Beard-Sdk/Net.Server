using Bb.ComponentModel;
using Bb.Extensions;
using Bb.Helpers;

namespace Bb.Servers.Web
{
    public static class ServiceRunnerExtensions
    {


        #region Manage starting urls and ports


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="startPorts">The starting range port</param>
        /// <param name="count">The count range port</param>
        /// <returns></returns>
        public static T AddLocalhostUrl<T>(this T self, string scheme, ref int startPorts, int count = 1)
            where T : ServiceRunnerBase
        {

            int _first = startPorts;

            List<string> ports = new List<string>();
            for (int i = 0; i < count; i++)
                self.AddLocalhostUrl(scheme, _first = HttpHelper.GetAvailablePort(_first) + 1);

            startPorts = _first;

            return self;

        }


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="hostName">The host name to listen</param>
        /// <param name="port">The port to listen.</param>
        /// <returns></returns>
        public static T AddUrl<T>(this T self, string scheme, string hostName, int port)
            where T : ServiceRunnerBase
        {

            self._urls.Add(scheme.ToUri(hostName, port));

            return self;
        }


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="port">The port to listen</param>
        /// <returns></returns>
        public static T AddLocalhostUrl<T>(this T self, string scheme, int port)
            where T : ServiceRunnerBase
        {
            self.AddUrl(scheme, "localhost", port);
            return self;
        }


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="host">The host name. if null localhost is used by default</param>
        /// <param name="startingPort">starting port to search.</param>
        /// <returns></returns>
        public static T AddLocalhostSecureUrlWithDynamicPort<T>(this T self, string? host, ref int startingPort)
            where T : ServiceRunnerBase
        {
            return self.AddLocalhostUrlWithDynamicPort("https", host, ref startingPort);
        }


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="host">The host name. if null localhost is used by default</param>
        /// <param name="startingPort">starting port to search.</param>
        /// <returns></returns>
        public static T AddLocalhostUrlWithDynamicPort<T>(this T self, string? host, ref int startingPort)
            where T : ServiceRunnerBase
        {
            return self.AddLocalhostUrlWithDynamicPort("http", host, ref startingPort);
        }


        /// <summary>
        /// Adds an <see cref="Url"/> on the list of listeners.
        /// </summary>
        /// <param name="scheme">The scheme protocol.</param>
        /// <param name="host">The host name. if null localhost is used by default</param>
        /// <param name="startingPort">starting port to search.</param>
        /// <returns></returns>
        public static T AddLocalhostUrlWithDynamicPort<T>(this T self, string scheme, string? host, ref int startingPort)
            where T : ServiceRunnerBase
        {

            var ports = self._urls.Select(c => c.Port).OrderBy(c => c).ToList();

            startingPort = HttpHelper.GetAvailablePort(startingPort);

            while (ports.Contains(startingPort))
            {
                startingPort++;
                startingPort = HttpHelper.GetAvailablePort(startingPort);
            }

            self.AddUrl(scheme, host ?? "localhost", startingPort);

            return self;
        }


        #endregion Manage starting urls and ports

    }

}
