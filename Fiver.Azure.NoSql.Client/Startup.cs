﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Fiver.Azure.NoSql.Client.OtherLayers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;

namespace Fiver.Azure.NoSql.Client
{
    public class Startup
    {
        public static IConfiguration Configuration;

        public Startup(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddScoped<IAzureNoSqlRepository<Movie>>(factory =>
            {
                return new AzureNoSqlRepository<Movie>(
                    new AzureNoSqlSettings(
                        endpoint: Configuration["NoSql_Endpoint"],
                        authKey: Configuration["NoSql_AuthKey"],
                        databaseId: Configuration["NoSql_Database"],
                        collectionId: Configuration["NoSql_Collection"]));
            });
            services.AddScoped<IMovieService, MovieService>();

            services.AddMvc();
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            //app.UseDeveloperExceptionPage();

            app.UseExceptionHandler(configure =>
            {
                configure.Run(async context =>
                {
                    var ex = context.Features
                                    .Get<IExceptionHandlerFeature>()
                                    .Error;
                    
                    await context.Response.WriteAsync($"Unexpected error: {ex.Message}");
                });
            });

            app.UseMvcWithDefaultRoute();
        }
    }
}
