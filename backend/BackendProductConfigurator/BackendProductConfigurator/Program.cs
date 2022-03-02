using BackendProductConfigurator.App_Code;
using Microsoft.Extensions.FileProviders;
using Model.Enumerators;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var MyAllowSpecificOrigins = "MyAllowSpecificOrigins";
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
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

GlobalValues.ServerAddress = builder.Configuration.GetValue<string>("ServerAddress");
GlobalValues.Ports = builder.Configuration.GetSection("DBPorts").Get<List<int>>().ToArray();
GlobalValues.Languages = builder.Configuration.GetSection("Languages").Get<List<string>>().ToArray();
GlobalValues.EmailServer = builder.Configuration.GetValue<string>("EmailServerAddress");
GlobalValues.ImagesFolder = builder.Configuration.GetValue<string>("ImagesFolder");
GlobalValues.PDFOutput = builder.Configuration.GetValue<string>("PdfOutput");
GlobalValues.ValueMode = (EValueMode)builder.Configuration.GetValue<int>("ValueMode");
GlobalValues.Secure = builder.Configuration.GetValue<bool>("Secure");
GlobalValues.MinutesBetweenFetches = builder.Configuration.GetValue<int>("MinutesBetweenFetches");
GlobalValues.TimeOut = builder.Configuration.GetValue<int>("TimeOut");


app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.UseStaticFiles();

app.Run();
