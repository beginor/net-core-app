using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GisHub.VectorTile.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GisHub.VectorTile {

    public class Program {

        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            // add configuration
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine("config", "appsettings.json"), true, true)
                .AddJsonFile(Path.Combine("config", $"appsettings.{builder.Environment.EnvironmentName}.json"), true, true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
            // config services;
            builder.Services
                .Configure<Dictionary<string, string>>(builder.Configuration.GetSection("connectionStrings"))
                .Configure<Dictionary<string, VectorTileSource>>(builder.Configuration.GetSection("vectors"))
                .AddSingleton<VectorTileProvider>()
                .AddSingleton(builder.Configuration.GetSection("cache").Get<CacheOptions>())
                .AddCors()
                .AddControllers();
            // build and config app
            var app = builder.Build();
            if (builder.Environment.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            var pathbase = GetAppPathbase();
            if (!string.IsNullOrEmpty(pathbase)) {
                app.UsePathBase(new PathString(pathbase));
                var message = "Hosting pathbase: " + pathbase;
                Console.WriteLine(message);
            }
            app.UseStaticFiles();
            app.UseCors(cors => {
                cors.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .DisallowCredentials();
            });
            app.UseRouting(); // tmp fix because pathbase is ignored.
            app.MapControllers();
            app.Run();
        }

        private static string GetAppPathbase() => Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");

    }
}
