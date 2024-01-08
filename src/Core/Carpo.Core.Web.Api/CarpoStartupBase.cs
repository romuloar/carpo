using Carpo.Core.Web.Api.Extensions;
using Carpo.Core.Web.Api.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Carpo.Core.Interface.ServiceLocator;
using Carpo.Core.UnitOfWork;

namespace Carpo.Core.Web.Api
{
    public class CarpoStartupBase : ICarpoStartup
    {
        public CarpoStartupBase(IConfiguration configuration)
        {            
            Configuration = configuration;
            NameApi = Configuration["CarpoConfig:NameSystem"];
        }

        public string NameApi
        {
            private set;
            get;
        }

        public string FullNameApiVersion
        {
            get { return NameApi + " - Version: " + VersionApi; }
        }

        public string VersionApi
        {
            get
            {
                string version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return version;
            }
        }

        public IConfiguration Configuration { get; }

        public virtual void Configure(WebApplication app, IWebHostEnvironment environment)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", FullNameApiVersion));

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var option = new RewriteOptions();
            option.AddRedirect("^$", "swagger");
            app.UseRewriter(option);
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IApplicationServiceLocator, ApplicationServiceLocator>();
            //todo ->
            //services.AddScoped<IRepositoryServiceLocator, UnitOfWorkRepositoryServiceLocator<CarpoContext>>();
            services.AddJwtBearerConfig(Configuration);
            services.AddSwaggerBearerConfig(NameApi, VersionApi);
            services.AddControllers();
        }
    }
}
