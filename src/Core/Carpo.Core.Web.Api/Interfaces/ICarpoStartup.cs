using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Carpo.Core.Web.Api.Interfaces
{
    public interface ICarpoStartup
    {
        IConfiguration Configuration { get; }
        void ConfigureServices(IServiceCollection services);
        void Configure(WebApplication app, IWebHostEnvironment environment);
        string VersionApi { get; }
        string FullNameApiVersion { get; }
    }
}
