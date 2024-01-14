using AutoMapper;
using Kinashy.ArrestSearchWebAPI.Apis;
using Kinashy.ArrestSearchWebAPI.Data;
using Kinashy.ArrestSearchWebAPI.Data.Auth;
using Kinashy.ArrestSearchWebAPI.Data.DTO;
using Kinashy.ArrestSearchWebAPI.Data.Library;
using Kinashy.ArrestSearchWebAPI.Data.RequiredProperties;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Oracle.ManagedDataAccess.Client;
using Samba;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
RegisterServices(builder.Services);
var app = builder.Build();
Configure(app);
app.MapGet("/", () => "Hello World!");
var apis = app.Services.GetServices<IApi>();
foreach (var api in apis)
{
    if (api is null) throw new InvalidProgramException("Api not Found");
    api.Register(app);
}
app.Run();

void RegisterServices(IServiceCollection services)
{
    services.AddTransient<IApi, LibraryApi>();
    services.AddTransient<IApi, AuthApi>();
    services.AddControllers()
            .AddJsonOptions(o => o.JsonSerializerOptions
                .ReferenceHandler = ReferenceHandler.Preserve);
    services.AddDbContext<LibraryDB>
        (
        options =>
        {

            options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite"));
            //    options.UseOracle(builder.Configuration.GetConnectionString("Oracle"), options => options
            //.UseOracleSQLCompatibility("11"));
        }
        );
    services.AddCors();
    services.AddSingleton<ITokenService>(new TokenService());
    services.AddSingleton<IUserRepository>(new UserRepository());
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
    services.AddAuthorization();
    services.AddScoped<ILibraryRepository, LibraryRepository>();
    services.AddAutoMapper(typeof(AppMappingProfile));
}
void Configure(WebApplication app)
{
    app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    );
        app.UseAuthentication();
        app.UseAuthorization();
        //if (app.Environment.IsDevelopment())
        //{
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<LibraryDB>();
        Console.WriteLine(db.Database.GenerateCreateScript());
        db.Database.EnsureCreated();
        //}
}