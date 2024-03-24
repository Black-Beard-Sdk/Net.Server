﻿using System.Text;

namespace Bb.Helpers
{


    public static class HttpHelper
    {


        public static Uri GetLocalUri(bool securised, int port = 0)
        {
            var uri = new UriBuilder();
            uri.Scheme = securised ? "https" : "http";
            uri.Host = "localhost";
            uri.Port = port;
            return uri.Uri;
        }

        public static Uri GetUri(bool securised, string host, int port = 0)
        {
            var uri = new UriBuilder();
            uri.Scheme = securised ? "https" : "http";
            uri.Host = host;
            uri.Port = port;
            return uri.Uri;
        }

        /// <summary>
        /// Gets the first available port.
        /// </summary>
        /// <param name="startingPort">The starting port.</param>
        /// <returns></returns>
        public static int GetAvailablePort(int startingPort)
        {

            var portArrayList = new List<int>();

            var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            // Ignore active connections
            var connections = properties.GetActiveTcpConnections();
            portArrayList.AddRange(from n in connections
                                   where n.LocalEndPoint.Port >= startingPort
                                   select n.LocalEndPoint.Port);

            // Ignore active tcp listeners
            var endPoints = properties.GetActiveTcpListeners();
            portArrayList.AddRange(from n in endPoints
                                   where n.Port >= startingPort
                                   select n.Port);

            // Ignore active UDP listeners
            endPoints = properties.GetActiveUdpListeners();
            portArrayList.AddRange(from n in endPoints
                                   where n.Port >= startingPort
                                   select n.Port);

            portArrayList.Sort();

            for (var i = startingPort; i < ushort.MaxValue; i++)
                if (!portArrayList.Contains(i))
                    return i;

            return -1;

        }

    }


}