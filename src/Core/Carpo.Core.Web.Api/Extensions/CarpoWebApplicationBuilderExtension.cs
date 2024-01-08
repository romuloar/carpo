using Carpo.Core.Web.Api.Interfaces;
using Microsoft.AspNetCore.Builder;

namespace Carpo.Core.Web.Api.Extensions
{
    public static class CarpoWebApplicationBuilderExtension
    {
        public static WebApplicationBuilder UseStartup<TStartup>(this WebApplicationBuilder webApplication) where TStartup : ICarpoStartup
        {
            var startup = Activator.CreateInstance(typeof(TStartup), webApplication.Configuration) as ICarpoStartup;
            if (startup == null) throw new ArgumentException("Invalid Startup class");

            startup.ConfigureServices(webApplication.Services);

            var app = webApplication.Build();
            startup.Configure(app, app.Environment);

            app.Run();

            return webApplication;
        }
    }
}
