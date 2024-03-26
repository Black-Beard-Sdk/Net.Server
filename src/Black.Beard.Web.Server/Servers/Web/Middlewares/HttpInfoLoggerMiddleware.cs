using Bb.Servers.Web.Models.Security;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics;
using System.Text;

namespace Bb.Servers.Web.Middlewares
{

    internal class HttpInfoLoggerMiddleware
    {

        public HttpInfoLoggerMiddleware(RequestDelegate next, ILogger<HttpInfoLoggerMiddleware> logger, IApiKeyRepository apiKeyRepository)
        {
            _next = next;
            _logger = logger;
            _apiKeyRepository = apiKeyRepository;
        }

        // RemoteIpAddress = ::1 is localhost
        // https://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core

        public async Task InvokeAsync(HttpContext context)
        {

            if (System.Diagnostics.Trace.CorrelationManager.ActivityId == Guid.Empty)
                System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();

            using (_logger.BeginScope(new[] {
                new KeyValuePair<string, object>("{url}", context.Request?.GetDisplayUrl()),
                new KeyValuePair<string, object>("{RemoteIpAddress}", context.Connection.RemoteIpAddress),
            }))
            {
                LogRequest(context);
                await _next(context);
                LogResponse(context);
            }

        }

        private void LogRequest(HttpContext context)
        {
            // Trace(context.Request);
            // var apiKeyOwner = _apiKeyRepository.GetApiKeyFromHeaders(context)?.Owner;
            //_logger.LogAction("Processing Request", () => _logger.LogRequest(context, apiKeyOwner));
        }

        private static void Trace(HttpRequest r)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(r.Method);
            sb.Append(" http");
            if (r.IsHttps)
                sb.Append("s");
            sb.Append("://");
            sb.Append(r.Host);
            sb.Append(r.Path);
            sb.Append(r.QueryString);

            sb.Append(" ");
            sb.AppendLine(r.Protocol);

            foreach (var item in r.Cookies)
                sb.AppendLine($"cookies: {item.Key}: {item.Value}");

            foreach (var item in r.Headers)
                sb.AppendLine($"Header: {item.Key}: {item.Value}");

            var form = TryToGetForm(r);
            if (form != null)
            {
                sb.AppendLine($"Form: ");

                if (form.Count > 0)
                    foreach (var item in form)
                    {
                        sb.AppendLine($"Key ({item.Key}) ");
                       var file = form.Files.Where(c => c.Name == item.Key).FirstOrDefault();
                        if (file != null)
                        {
                            sb.AppendLine($"    File:");
                            sb.AppendLine($"      Name: {file.Name}");
                            sb.AppendLine($"      FileName: {file.FileName}");
                            sb.AppendLine($"      Length: {file.Length}");
                            foreach (var item2 in file.Headers)
                                sb.AppendLine($"    Header : {item2.Key}: {item2.Value}");
                        }
                        else
                            sb.AppendLine($"value ({item.Value}) ");
                    }
                else
                    foreach (var item in form.Files)
                    {
                        sb.AppendLine($"    File:");
                        sb.AppendLine($"      Name: {item.Name}");
                        sb.AppendLine($"      FileName: {item.FileName}");
                        sb.AppendLine($"      Length: {item.Length}");
                        foreach (var item2 in item.Headers)
                            sb.AppendLine($"    Header : {item2.Key}: {item2.Value}");
                    }

            }

            var txt = sb.ToString();
        }

        private static IFormCollection? TryToGetForm(HttpRequest r)
        {
            IFormCollection form = null;
            try
            {
                if (r.HasFormContentType && r.Form != null)
                    form = r.Form;
            }
            catch (Exception)
            {
            }
            return form;
        }

        private void LogResponse(HttpContext context)
        {
            // _logger.LogAction("Returning Response", () => _logger.LogResponse(context));
        }

        private readonly RequestDelegate _next;
        private readonly ILogger<HttpInfoLoggerMiddleware> _logger;
        private readonly IApiKeyRepository _apiKeyRepository;

    }

}


