using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;

namespace idsserver
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectStr = Configuration.GetConnectionString("DefaultConnection");
            
            services.AddIdentityServer()
                .AddInMemoryApiScopes(new List<ApiScope> {
                    new ApiScope("weatherapi.read", "Read Weather API"),
                })
                .AddInMemoryApiResources(new List<ApiResource>() {
                    new ApiResource("weatherapi") {
                        Scopes = { "weatherapi.read" },
                    }
                })
                .AddInMemoryClients(new List<Client> {
                    new Client
                    {
                        ClientId = "m2m.client",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        ClientSecrets = { new Secret("SuperSecretPassword".Sha256())},
                        AllowedScopes = { "weatherapi.read" }
                    },
                    new Client
                    {
                        ClientId = "interactive",

                        AllowedGrantTypes = GrantTypes.Code,
                        RequireClientSecret = false,
                        RequirePkce = true,

                        RedirectUris = { "http://localhost:3000/signin-oidc" },
                        PostLogoutRedirectUris = { "http://localhost:3000" },

                        AllowedScopes = { "openid", "profile", "weatherapi.read" }
                    },
                })
                .AddInMemoryIdentityResources(new List<IdentityResource>() {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                })
                .AddTestUsers(new List<TestUser>() {
                    new TestUser
                        {
                            SubjectId = "Alice",
                            Username = "alice",
                            Password = "alice"
                        }
                });


            // add views
            services.AddControllersWithViews();

            // add CORS
            services.AddCors();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // use Cors
            app.UseCors(config => config
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseRouting();
            app.UseIdentityServer();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
