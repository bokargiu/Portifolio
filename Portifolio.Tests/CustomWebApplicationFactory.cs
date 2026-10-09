using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portifolio.Server.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portifolio.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) => { 
                config.AddInMemoryCollection(new Dictionary<string, string?> { 
                    ["Jwt:Key"] = "ChaveDeTesteSeguraComPeloMenos32Bytes!", 
                    ["Jwt:Issuer"] = "auth-web", 
                    ["Jwt:Audience"] = "Client" });
            });

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DB>();
                services.RemoveAll<DbContextOptions<DB>>();
                services.RemoveAll<IDbContextOptionsConfiguration<DB>>();

                services.AddDbContext<DB>(options =>
                {
                    options.UseInMemoryDatabase($"PortifolioTests_{Guid.NewGuid()}");
                });
            });
        }
    }
}
