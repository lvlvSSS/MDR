
using MDR.Server.Middleware;
using Microsoft.AspNetCore.HttpLogging;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpLogging(builder =>
{
    builder.LoggingFields = HttpLoggingFields.All;
    builder.RequestHeaders.Add("My-Request-Header");
    builder.ResponseHeaders.Add("My-Response-Header");
    builder.MediaTypeOptions.AddText("application/javascript");
    builder.RequestBodyLogLimit = 4096;
    builder.ResponseBodyLogLimit = 4096;
});
builder.Services.AddLogging(builder =>
{
    builder.ClearProviders();
    builder.SetMinimumLevel(LogLevel.Information);
    builder.AddNLog("NLog.config");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseHttpLogging();
// needed for HTTP response body with an API Controller.
app.UseMiddleware<NLogResponseBodyMiddleware>(new NLogResponseBodyMiddlewareOptions());
app.MapControllers();


app.Run();
