
using Demo.Insights.Configuration;
using Serilog;

namespace Demo.Insights
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddOTelTracing(builder.Configuration);
            builder.Services.AddOTelMetrics(builder.Configuration);
            builder.Logging.AddOTelLogging(builder.Configuration);

            builder.Host.UseSerilog((ctx, conf) =>
                conf.ReadFrom.Configuration(ctx.Configuration));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
