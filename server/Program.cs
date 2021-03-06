﻿using System;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using server.Model;
using Microsoft.EntityFrameworkCore;

namespace server
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(string[] args) =>

            Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(
                webBuilder => { webBuilder.UseStartup<Startup>(); }
            );


    }
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        { 
            string condb = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<bdContext>(options =>
                options.UseSqlite(condb));
            services.AddSignalR();
        }
        public void Configure(IApplicationBuilder application, IWebHostEnvironment env)
        {
            application.UseRouting();
            application.UseEndpoints(endpoint =>
            {
                endpoint.MapHub<MasterHub>("/master");
            });
        }
    }
}
