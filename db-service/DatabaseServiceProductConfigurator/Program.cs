using DatabaseServiceProductConfigurator.Models;
using DatabaseServiceProductConfigurator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// CORS

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options => {
    options.AddPolicy(name: MyAllowSpecificOrigins,
        corsBuilder => {
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
        }
    );
});

// Connection String
string activeDb = builder.Configuration.GetValue<string>("activeDB");
builder.Services.AddDbContext<Product_configuratorContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("default"), Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.22-mysql"))
);

// Services
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() ) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();