using Demo.Seq.Configuration;
using Demo.Seq.Services;
using Serilog;

namespace Demo.Seq
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient<IWeatherService, WeatherService>(config =>
                config.BaseAddress = new Uri("https://localhost:7002"));

            builder.Host.UseSerilog((ctx, conf) =>
                conf.ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddOTelConfiguration(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
