using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Demo.Insights.Configuration
{
    public static class OTelExtensions
    {
        public static void AddOTelTracing(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                 .ConfigureResource(resource => resource.AddService("Demo.Insights"))
                 .WithTracing(tracing =>
                 {
                     tracing
                     .AddAspNetCoreInstrumentation()
                     .AddHttpClientInstrumentation()
                     .AddAzureMonitorTraceExporter(c =>
                        c.ConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
                 });
        }

        public static void AddOTelMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOpenTelemetry()
                  .ConfigureResource(resource => resource.AddService("Demo.Insights"))
                  .WithMetrics(metrics =>
                  {
                      metrics
                      .AddAspNetCoreInstrumentation()
                      .AddHttpClientInstrumentation()
                      .AddAzureMonitorMetricExporter(c =>
                         c.ConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
                  });
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
