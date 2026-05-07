
using GameServer.Databases.DbContexts;
using GameServer.Handlers;
using GameServer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pray_server.Hosts;
using Serilog;
using StackExchange.Redis;

namespace GameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });

            // dbfactory regist
            builder.Services
                .AddDbContextFactory<AccountDbContext>(options =>
                    options
                        .UseSqlServer(builder.Configuration.GetConnectionString("AccountConnection"))
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking))
                .AddDbContextFactory<GameDbContext>(options =>
                    options
                        .UseSqlServer(builder.Configuration.GetConnectionString("GameConnection"))
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            // Add services to the container.
            builder.Services.AddControllers();

            var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
            builder.Services.AddSingleton<IConnectionMultiplexer>(
                ConnectionMultiplexer.Connect(redisConnection));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // manager register
            ServiceCollectionRegister.Register(builder.Services);

            //controltower register
            if (!builder.Environment.EnvironmentName.StartsWith("alpha")
                || !builder.Environment.EnvironmentName.StartsWith("live"))
            {
                builder.Services.AddHostedService<ServerRegistHost>();
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || builder.Environment.EnvironmentName.StartsWith("dev"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=action}/{action=Index}/{className}/{packetName}"
            );

            app.Run();
        }
    }
}
