using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.ExceptionHandler;
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
string connectionString = builder.Configuration.GetConnectionString(activeDb);

builder.Services.AddDbContext<ConfiguratorContext>(options => 
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// Services
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IRuleService, RuleService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IProductService, ProductService>();

// Add services to the container.

builder.Services.AddControllers(options => {
    options.Filters.Add<HttpResponseExceptionFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() ) {
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error-development");
}
else {
    app.UseExceptionHandler("/error");
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();