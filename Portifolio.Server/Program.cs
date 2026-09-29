using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portifolio.Server.Database;
using Portifolio.Server.Services.AuthServices;
using Portifolio.Server.Services.Project_Services;
using Portifolio.Server.Services.TechInfo_Services;
using Portifolio.Server.Services.User_Services;
using System.Text;

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

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
