using BackendProductConfigurator.App_Code;
using Microsoft.Extensions.FileProviders;
using Model.Enumerators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        corsBuilder =>
        {
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
        }
    );
});

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

GlobalValues.ServerAddress = builder.Configuration.GetValue<string>("ServerAddress");
GlobalValues.ValueMode = (EValueMode)builder.Configuration.GetValue<int>("ValueMode");
GlobalValues.Secure = builder.Configuration.GetValue<bool>("Secure");
GlobalValues.MinutesBetweenFetches = builder.Configuration.GetValue<int>("MinutesBetweenFetches");

app.UseFileServer(
    new FileServerOptions
    {
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "FrontEnd")),
        RequestPath = ""
    }
    );

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
