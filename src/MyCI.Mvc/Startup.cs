using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCI.Mvc.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace MyCI.Mvc
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddMvcCore()
                .SetJsonFormatter();
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .RemovePlainFormatter()
                .SetAuthorizePage();
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "MyCI API", Version = "v1"}); });

            services.Configure<MyCISettings>(Configuration.GetSection("MyCISettings"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
//https://medium.com/@nicolastakashi/asp-net-core-api-behind-the-nginx-reverse-proxy-with-docker-72eeccfb5063
            //this: route incoming headers through the request that arrived at Nginx for our ASP.NET service.
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            //Enable swagger
            app.UseSwagger(c =>
                //RouteTemplate cannot start with a '/' !
                c.RouteTemplate = "swagger/{documentName}/swagger.json");

            //Enable Swagger UI
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api-swagger";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyCI API V1");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}