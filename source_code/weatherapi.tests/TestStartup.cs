using API.Tests.PactSetup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using weatherapi;

namespace API.Tests
{
    public class TestStartup : Startup // this is the real WebAPI startup
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