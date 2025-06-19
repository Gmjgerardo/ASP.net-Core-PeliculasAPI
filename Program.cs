using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI;
using PeliculasAPI.Services;
using PeliculasAPI.Utilities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Obtaining AllowedCORS
var AllowedCORS = builder.Configuration.GetValue<string>("AllowedCORS")!.Split(";");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Añadiendo funcionalidad de cache
builder.Services.AddOutputCache(options => {options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);});

builder.Services.AddCors(options => options.AddDefaultPolicy(opts =>
{
    opts.WithOrigins(AllowedCORS)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders(["total-records-count"]);
}));

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), o => o.UseNetTopologySuite())
);

// Añadiendo funcionalidad de administración de usuarios básica de dotnet
builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDBContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();
builder.Services.AddAuthentication().AddJwtBearer(opts =>
{
    opts.MapInboundClaims = false;
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));

builder.Services.AddSingleton(provider => new MapperConfiguration(conf =>
{
    GeometryFactory geometryFactory = provider.GetRequiredService<GeometryFactory>();
    conf.AddProfile(new AutoMapperProfiles(geometryFactory));
}).CreateMapper());

builder.Services.AddTransient<IFileStorage,LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

app.Run();
