using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portifolio.Server.Database;
using Portifolio.Server.Services.AuthServices;
using Portifolio.Server.Services.Project_Services;
using Portifolio.Server.Services.TechInfo_Services;
using Portifolio.Server.Services.User_Services;
using System.Security.Claims;
using System.Text;
using System.Net;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSwaggerGen();
#region Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
#endregion
#region Database
var connectionString = builder.Configuration["ConnectionStrings:Connection"];
builder.Services.AddDbContext<DB>(options =>
{
    options.UseMySQL(connectionString);
});
#endregion
#region Jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
    });
#endregion
#region Volume Medias
var mediaPath = Path.Combine(builder.Environment.ContentRootPath, "media-portifolio");

Directory.CreateDirectory(mediaPath);
#endregion
#region Rate Limiter
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownProxies.Add(
        IPAddress.Parse("187.75.67.81")
    );
});

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("BruteForceProtection", opt =>
    {
        opt.TokenLimit = 4;
        opt.ReplenishmentPeriod = TimeSpan.FromMinutes(1);
        opt.TokensPerPeriod = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        httpContext =>
        {
            var user = httpContext.User;

            if (user.Identity?.IsAuthenticated == true)
            {
                var isAdmin = user.IsInRole("Admin");

                var userId = user.FindFirst("PrimarySid")?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    userId = "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: userId,
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = isAdmin ? 30 : 10,
                        QueueLimit = 0,
                        Window = TimeSpan.FromSeconds(30),
                        AutoReplenishment = true
                    });
            }

            return RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 10,
                    TokensPerPeriod = 5,
                    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                });
        });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
#endregion
builder.Services.AddHttpContextAccessor();
#region Services Scoped
builder.Services.AddScoped<DB>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITechInfoService, TechInfoService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
#endregion
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapGet("/api/", () => "API está funcionando!");

app.UseCors("AllowAll");
app.UseForwardedHeaders();
app.UseDefaultFiles();
app.MapStaticAssets();
using (var scope = app.Services.CreateScope())
{ // Adicionando Migrações
    var db = scope.ServiceProvider.GetRequiredService<DB>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
