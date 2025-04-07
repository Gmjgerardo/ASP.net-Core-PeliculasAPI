using Microsoft.EntityFrameworkCore;
using PeliculasAPI;

var builder = WebApplication.CreateBuilder(args);

// Obtaining AllowedCORS
var AllowedCORS = builder.Configuration.GetValue<string>("AllowedCORS")!.Split(";");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// A�adiendo funcionalidad de cache
builder.Services.AddOutputCache(options => {options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);});

builder.Services.AddCors(options => options.AddDefaultPolicy(opts =>
{
    opts.WithOrigins(AllowedCORS).
        AllowAnyMethod().
        AllowAnyHeader();
}));

builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

app.UseCors();

app.Run();
