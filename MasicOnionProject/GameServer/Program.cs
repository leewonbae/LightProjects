
using BattleServer.Helpers;
using MagicOnion.Server;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using Serilog;
using Snowpipe.Commons.BattleServerCommons;

namespace BattleServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Host.UseSerilog((context, config) =>
            {
                config.ReadFrom.Configuration(context.Configuration);
            });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First())
            );

            builder.Services.AddMagicOnion();

            builder.WebHost.ConfigureKestrel(options =>
            {
                // HTTP 전용
                options.ListenAnyIP(5102, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });

                // gRPC 전용
                options.ListenAnyIP(5112, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });


            });

            BattlePacketResistry.Init();
            ServiceCollectionRegister.Register(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();
            app.MapMagicOnionService();

            app.Run();
        }
    }
}
