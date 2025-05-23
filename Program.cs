using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Http.Features;
using TLRProcessor.Jobs;
using TLRProcessor.Repositories;
using TLRProcessor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISmsTlrRepository, SmsTlrRepository>();
builder.Services.AddScoped<LargeFileProcessor>();
builder.Services.AddScoped<SmsTlrJob>();

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1_000_000_000; // 1 GB
   
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TLR API V1");
    c.RoutePrefix = string.Empty;
});
app.UseAuthorization();
app.MapControllers();
app.UseHangfireDashboard();
app.Run();