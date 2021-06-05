using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using weatherapi;

namespace weatherapi.tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment webHost)
        {
            app.UseMiddleware<ProviderStateMiddleware>();
            base.Configure(app, webHost);
        }
    }

}