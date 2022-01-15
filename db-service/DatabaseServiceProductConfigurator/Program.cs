//using DatabaseServiceProductConfigurator.Models;

using DatabaseServiceProductConfigurator.Models;
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