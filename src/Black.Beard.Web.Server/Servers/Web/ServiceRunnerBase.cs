using Bb.Extensions;
using NLog;
using NLog.Web;
using System.Collections;
using System.Diagnostics;
using System.Text;
using Bb.Servers.Web.Models;
using Bb.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Bb.Servers.Web
{

    public class ServiceRunnerBase
    {


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceRunner{TStartup}"/> class.
        /// </summary>
        /// <param name="start">if set to <c>true</c> [start].</param>
        /// <param name="args">The arguments.</param>
        public ServiceRunnerBase(params string[] args)
        {
            _args = args;
            _tokenSource = new CancellationTokenSource();
            _token = _tokenSource.Token;
            Console.CancelKeyPress += Console_CancelKeyPress;
            Logger = InitializeLogger();
        }

        /// <summary>
        /// Gets the status of the service.
        /// </summary>
        public ServiceRunnerStatus Status { get; protected set; }


        /// <summary>
        /// Gets the exit code.
        /// </summary>
        /// <value>
        /// The exit code.
        /// </value>
        public int ExitCode { get; protected set; }

        /// <summary>
        /// Gets the build host.
        /// </summary>
        /// <value>
        /// The build.
        /// </value>
        public IHost? Build { get; protected set; }

        /// <summary>
        /// Gets the addresses service listener.
        /// </summary>
        /// <value>
        /// The addresses.
        /// </value>
        public List<Uri>? Addresses { get; protected set; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        public Logger Logger { get; }

        /// <summary>
        /// Gets the runner task.
        /// </summary>
        /// <value>
        /// The runner.
        /// </value>
        public Task Task => _task;

        /// <summary>
        /// Cancels the running instance.
        /// </summary>
        public virtual bool CancelAsync()
        {

            var task = Build.StopAsync();

            try
            {

                if (_task != null && _tokenSource != null && _tokenSource.Token.CanBeCanceled)
                    _tokenSource.Cancel();

                //task.Wait(_token);

                var timeOut = DateTime.Now.AddMinutes(1);
                while (Status != ServiceRunnerStatus.Stopped && timeOut > DateTime.Now) Task.Yield();

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }

            return Status == ServiceRunnerStatus.Stopped;

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            CancelAsync();
            Console.CancelKeyPress -= Console_CancelKeyPress;
            _tokenSource?.Dispose();
        }

        /// <summary>
        /// Waits for the <see cref="Task"/> to complete execution.
        /// </summary>
        /// <exception cref="AggregateException">
        /// The <see cref="Task"/> was canceled -or- an exception was thrown during
        /// the execution of the <see cref="Task"/>.
        /// </exception>
        public void Wait()
        {
            _task?.Wait(_token);
        }

        /// <summary>
        /// Waits for the <see cref="Task"/> to complete execution.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait, or <see cref="Timeout.Infinite"/> (-1) to
        /// wait indefinitely.</param>
        /// <returns>true if the <see cref="Task"/> completed execution within the allotted time; otherwise,
        /// false.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="millisecondsTimeout"/> is a negative number other than -1, which represents an
        /// infinite time-out.
        /// </exception>
        /// <exception cref="AggregateException">
        /// The <see cref="Task"/> was canceled -or- an exception was thrown during the execution of the <see
        /// cref="Task"/>.
        /// </exception>
        public void Wait(int millisecondsTimeout)
        {
            _task?.Wait(millisecondsTimeout);
        }

        /// <summary>
        /// Runs asynchronous service
        /// </summary>
        /// <param name="waitRunning">if set to <c>true</c> [wait service running].</param>
        /// <returns></returns
        public virtual async Task RunAsync(bool waitRunning = true)
        {

            Status = ServiceRunnerStatus.Launching;

            await Task.Run(() => Run(), _token);

            if (waitRunning)
                while (Status != ServiceRunnerStatus.Running)
                    await Task.Yield();



        }

        /// <summary>
        /// wait the predicate is true before continue
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task Wait(Func<ServiceRunnerBase, bool> predicate)
        {

            while (predicate(this))
                await Task.Yield();

        }

        /// <summary>
        /// Runs this instance and wait closing.
        /// </summary>
        public virtual void Run()
        {

            Status = ServiceRunnerStatus.Preparing;

            Build
                = CreateHostBuilder(Logger, _args)
                 .Build()
                ;

            Trace.TraceInformation("Current directory : " + Directory.GetCurrentDirectory());

            try
            {

                _task = RunAsync(Build);

                if (_exception != null)
                    throw _exception;

                EnumerateListeners();

                Status = ServiceRunnerStatus.Running;

                ServiceRunning();


                    _task?.Wait();

                ExitCode = 0;

                Status = ServiceRunnerStatus.Stopped;

            }
            catch (Exception exception)
            {
                Status = ServiceRunnerStatus.Stopped;
                ExitCode = exception.HResult;
                Logger.Error(exception, "Stopped program because of exception");

                if (Debugger.IsAttached)
                    Debugger.Break();

            }
            finally
            {
                Status = ServiceRunnerStatus.Stopped;
                LogManager.Shutdown();
                Environment.ExitCode = ExitCode;
            }

        }

        /// <summary>
        /// Interceptor for catch the service running
        /// </summary>
        protected virtual void ServiceRunning()
        {



        }

        protected virtual NLogAspNetCoreOptions ConfigureNlog()
        {
            return new NLogAspNetCoreOptions()
            {
                IncludeScopes = true,
                IncludeActivityIdsWithBeginScope = true,
            };
        }

        protected virtual void ConfigureLogging(ILoggingBuilder builder)
        {
            builder.ClearProviders()
           ;
        }

        protected virtual void SetConfiguration(Logger logger, WebHostBuilderContext hostingContext, IConfigurationBuilder config)
        {


            GlobalConfiguration.IsDevelopment = hostingContext.HostingEnvironment.IsDevelopment();
            GlobalConfiguration.IsProduction = hostingContext.HostingEnvironment.IsProduction();
            GlobalConfiguration.IsStaging = hostingContext.HostingEnvironment.IsStaging();

            // Load configurations files
            new ConfigurationLoader(logger, hostingContext, config)
                 .TryToLoadConfigurationFile("appsettings.json", false, false)
                 .TryToLoadConfigurationFile("apikeysettings.json", false, false)
                 .TryToLoadConfigurationFile("policiessettings.json", false, false)
             ;

            config.AddEnvironmentVariables();
            config.AddCommandLine(_args);

        }

        protected virtual void TuneHostBuilder(IWebHostBuilder webBuilder)
        {

        }

        protected virtual Logger InitializeLogger()
        {

            // target folder where write
            GlobalDiagnosticsContext.Set("web_log_directory", GlobalConfiguration.DirectoryToTrace);

            // push environment variables in the log
            foreach (DictionaryEntry item in Environment.GetEnvironmentVariables())
                if (item.Key != null
                    && !string.IsNullOrEmpty(item.Key.ToString())
                    && item.Key.ToString().StartsWith("web_log_"))
                    GlobalDiagnosticsContext.Set(item.Key.ToString(), item.Value?.ToString());

            // load the configuration file
            var configLogPath = Directory.GetCurrentDirectory().Combine("nlog.config");
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(configLogPath);

            // Initialize log
            var logger = LogManager
                .Setup()
                .SetupExtensions(s => { })
                .GetCurrentClassLogger()
                ;

            logger.Debug("log initialized");

            return logger;
        }




        private IHostBuilder CreateHostBuilder(Logger logger, string[] args)
        {

            //InitializeByOs();

            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {

                    if (_urls != null)
                        webBuilder.UseUrls(ConcatUrl(_urls).ToString());

                    webBuilder.ConfigureAppConfiguration((hostingContext, config) => SetConfiguration(logger, hostingContext, config));

                    webBuilder.ConfigureLogging(l => ConfigureLogging(l))
                              .UseNLog(ConfigureNlog());

                    TuneHostBuilder(webBuilder);

                });

            return hostBuilder;

        }

        private async Task RunAsync(IHost host, CancellationToken token = default)
        {
            try
            {
                Status = ServiceRunnerStatus.Starting;
                await host.StartAsync(token).ConfigureAwait(false);
                await host.WaitForShutdownAsync(token).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.Error(e);
                _exception = e;
            }
            finally
            {
                Status = ServiceRunnerStatus.Stopping;
                if (host is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync().ConfigureAwait(false);
                }
                else
                {
                    host.Dispose();
                }
            }
        }

        private void EnumerateListeners()
        {

            var addresses = Build.GetServerAcceptedAddresses();
            foreach (var address in addresses)
            {
                Trace.TraceInformation($"address : {address}");
                Logger.Info($"address : {address}");
            }

            Addresses = addresses;

        }

        /// <summary>
        /// return a <see cref="StringBuilder"/> with Concatenated url separated by ';'.
        /// </summary>
        /// <param name="urls"><see cref="Url"/></param>
        /// <returns></returns>
        public static StringBuilder ConcatUrl(IEnumerable<Uri> urls)
        {
            return ConcatUrl(new StringBuilder(), urls);
        }

        /// <summary>
        /// return a <see cref="StringBuilder"/> with Concatenated url separated by ';'.
        /// </summary>
        /// <param name="sb"><see cref="StringBuilder"/></param>
        /// <param name="urls"><see cref="Url"/></param>
        /// <returns></returns>
        public static StringBuilder ConcatUrl(StringBuilder sb, IEnumerable<Uri> urls)
        {
            if (sb == null)
                sb = new StringBuilder(200);

            string comma = string.Empty;
            foreach (var url in urls)
            {
                sb.Append(comma);
                sb.Append(url.ToString().ToLower());
                comma = ";";
            }

            return sb;

        }

        private void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            CancelAsync();
        }


        private Task? _task;
        private Exception? _exception;
        protected List<Uri>? _urls;
        private readonly string[] _args;
        private readonly CancellationTokenSource _tokenSource;
        private readonly CancellationToken _token;

    }


}
