using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Minesweeper.Model;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Minesweeper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Data access
            services.AddDbContext<MinesweeperContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("Minesweeper")));

            // Authn
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<MinesweeperContext>();
            services.AddIdentityServer()
                .AddApiAuthorization<IdentityUser, MinesweeperContext>();
            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddMvcOptions(opts => opts.EnableEndpointRouting = false)
                .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Use the Swagger Generator to describe our API
            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Minesweeper",
                    Contact = new OpenApiContact()
                    {
                        Name = "Alexis Yannuzzi",
                        Email = "alexisy@turboserver.com.ar"
                    }
                });
                opts.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                // TODO: make endpoint paths camelCase (https://github.com/domaindrivendev/Swashbuckle.WebApi/issues/834, seemengly fixed in aspnet v2.2
                
                // payload is supposed to be serialized in camelCase already (test that!), but the doc doesn't know it so nudge it
                // TODO: This doesn't work :(
                opts.DescribeAllParametersInCamelCase();

                opts.UseInlineDefinitionsForEnums();
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped<IGameRepository, DbGameRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            // Serve the generated swagger document
            app.UseSwagger();
            // Plus the fancy UI (at /swagger)
            app.UseSwaggerUI();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
