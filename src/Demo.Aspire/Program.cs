using Demo.Aspire.Configuration;
using Demo.Aspire.Services;

namespace Demo.Aspire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddIdentityAndDbContext(builder.Configuration);
            builder.Services.AddOTelTracing(builder.Configuration);
            //builder.Services.AddOTelMetrics(builder.Configuration);
            builder.Logging.AddOTelLogging(builder.Configuration);

            builder.Services.AddHttpClient<IWeatherService, WeatherService>(config =>
                config.BaseAddress = new Uri("https://localhost:7002"));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
