using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Demo.Aspire.Configuration
{
    public static class OTelExtensions
    {
        public static void AddOTelTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = typeof(Program).Assembly.GetName().Name ??
                throw new InvalidOperationException("Assembly name could not be determined.");

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithTracing(tracing =>
                {
                    tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                    if (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
                        tracing.AddOtlpExporter();
                });
        }

        public static void AddOTelMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = typeof(Program).Assembly.GetName().Name ??
                throw new InvalidOperationException("Assembly name could not be determined.");

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithMetrics(metrics =>
                {
                    metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                    if (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
                        metrics.AddOtlpExporter();
                });
        }

        public static void AddOTelLogging(this ILoggingBuilder builder, IConfiguration configuration)
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;

                if (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
                    logging.AddOtlpExporter();
            });
        }
    }
}
