using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Docs.Extensions;
using MudBlazor.Examples.Data;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace MudBlazor.Docs.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPeriodicTableService, PeriodicTableService>();
            services.AddScoped(sp => new HttpClient() { BaseAddress = new Uri(Configuration["ApiBase"]) });
            services.AddHeadElementHelper();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.TryAddDocsViewServices();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // serve the wasm site and finish the pipeline
            app.MapWhen(ctx => ctx.Request.Path.StartsWithSegments("/wasm"), wasm =>
            {
                wasm.UseBlazorFrameworkFiles("/wasm");
                wasm.UseStaticFiles("/wasm");
                wasm.UseRouting();
                wasm.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapFallbackToFile("wasm/{*path:nonfile}", "wasm/index.html");
                });
            });

            app.UseHeadElementServerPrerendering();

            // only reach here if pasth does not start /wasm
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
