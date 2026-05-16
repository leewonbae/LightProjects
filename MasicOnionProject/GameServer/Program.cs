using GameServer.Helpers;
using MagicOnion.Serialization.MessagePack;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

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

            // Add services to the container.
            builder.Services.AddControllers();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //magicOnion 

            builder.Services.AddMagicOnion(options =>
            {
                options.MessageSerializer = MessagePackMagicOnionSerializerProvider.Default;
            });

            builder.WebHost.ConfigureKestrel(options =>
            {
                // REST + Swagger ¡æ HTTP/1.1 
                options.ListenLocalhost(5001, o =>
                {
                    o.Protocols = HttpProtocols.Http1;
                });

                // gRPC Àü¿ë ¡æ HTTP/2 only
                options.ListenLocalhost(5002, o =>
                {
                    o.Protocols = HttpProtocols.Http2;
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || builder.Environment.EnvironmentName.StartsWith("dev"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=action}/{action=Index}/{className}/{packetName}"
            );

            app.MapMagicOnionService();

            app.Run();
        }
    }
}
