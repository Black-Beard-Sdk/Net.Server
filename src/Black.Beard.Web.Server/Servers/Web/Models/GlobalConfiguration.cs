using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Extensions;

namespace Bb.Servers.Web.Models
{


    [ExposeClass(Context = ConstantsCore.Configuration, LifeCycle = IocScopeEnum.Singleton)]
    public class GlobalConfiguration
    {

        public GlobalConfiguration()
        {

            UseSwagger = "use_swagger".EnvironmentVariableExists()
                ? "use_swagger".EnvironmentVariableIsTrue()
                : IsDevelopment;

            UseSwagger = "use_controller".EnvironmentVariableExists()
                ? "use_controller".EnvironmentVariableIsTrue()
                : true;

            TraceAll = "trace_all".EnvironmentVariableExists()
                ? "trace_all".EnvironmentVariableIsTrue()
                : IsDevelopment;

            UseTelemetry = "use_telemetry".EnvironmentVariableExists()
                ? "use_telemetry".EnvironmentVariableIsTrue()
                : IsDevelopment;

        }



        /// <summary>
        /// Gets or sets a value indicating whether [trace all] (verbose mode).
        /// </summary>
        /// <value>
        ///   <c>true</c> if [trace all]; otherwise, <c>false</c>.
        /// </value>
        public bool TraceAll { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use swagger].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use swagger]; otherwise, <c>false</c>.
        /// </value>
        public bool UseSwagger { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use controllers].
        /// </summary>
        public bool UseControllers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use telemetry].
        /// </summary>
        public bool UseTelemetry { get; set; }



        /// <summary>
        /// Gets or sets the current directory to write generators.
        /// </summary>
        /// <value>
        /// The current directory to write generators.
        /// </value>
        public static string? CurrentDirectoryToWriteGenerators { get; set; }

        /// <summary>
        /// Gets or sets the trace log to write.
        /// </summary>
        /// <value>
        /// The trace log to write.
        /// </value>
        public static string? DirectoryToTrace { get; set; }




        public static bool IsDevelopment { get; internal set; }

        public static bool IsProduction { get; internal set; }

        public static bool IsStaging { get; internal set; }

    }


}
