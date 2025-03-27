using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Demo.Seq.UnderlyingService.Configuration
{
    public static class OTelExtensions
    {
        public static void AddOTelConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = typeof(Program).Assembly.GetName().Name ??
                throw new InvalidOperationException("Assembly name could not be determined.");

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(serviceName))
                .WithMetrics(builder =>
                {
                    builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                    if (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
                        builder.AddOtlpExporter();
                })
                .WithTracing(builder =>
                {
                    builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                    if (configuration["OTEL_EXPORTER_OTLP_ENDPOINT"] != null)
                        builder.AddOtlpExporter();
                });
        }
    }
}
