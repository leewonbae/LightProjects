
using GameServer.Databases.DbContexts;
using GameServer.Handlers;
using GameServer.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // manager register
            ServiceCollectionRegister.Register(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
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
