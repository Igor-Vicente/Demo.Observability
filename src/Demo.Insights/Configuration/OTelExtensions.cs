using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Demo.Insights.Configuration
{
    public static class OTelExtensions
    {
        public static void AddOTelTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var otel = services.AddOpenTelemetry()
                 .ConfigureResource(resource => resource.AddService("Demo.Insights"))
                 .WithTracing(tracing =>
                 {
                     tracing
                     .AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation();
                 });

            if (!string.IsNullOrEmpty(configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
                otel.UseAzureMonitor();
        }

        public static void AddOTelMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var otel = services.AddOpenTelemetry()
                 .ConfigureResource(resource => resource.AddService("Demo.Insights"))
                 .WithMetrics(metrics =>
                 {
                     metrics
                     .AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation();
                 });

            if (!string.IsNullOrEmpty(configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
                otel.UseAzureMonitor();

        }

        public static void AddOTelLogging(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.AddOpenTelemetry(logging =>
             {
                 logging.IncludeFormattedMessage = true;
                 logging.IncludeScopes = true;
             });


        }
    }
}
