using System;
using System.Collections.Generic;
using Serilog.Core;
using Serilog.Events;

namespace ApiParser.Logging
{
    public class HttpClientLoggingFilter : ILogEventFilter
    {
        private static readonly HashSet<string> IgnoredMessages = new HashSet<string>(StringComparer.Ordinal)
        {
            "Start processing HTTP request {HttpMethod} {Uri}",
            "Received HTTP response after {ElapsedMilliseconds}ms - {StatusCode}",
            "End processing HTTP request after {ElapsedMilliseconds}ms - {StatusCode}",
            "Sending HTTP request {HttpMethod} {Uri}"
        };

        // Allow the event to be logged if the message template isn't one we ignore
        public bool IsEnabled(LogEvent logEvent) => !IgnoredMessages.Contains(logEvent.MessageTemplate.Text);
    }
}