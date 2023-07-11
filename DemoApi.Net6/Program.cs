using DemoApi.Net6;
using Prometheus;
using Hei.Infrastructure;
using Nacos.V2.Naming.Dtos;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Configuration;
using Hei.Hystrix;
using DemoApi.Net6.Services;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.local.json", reloadOnChange: true, optional: true);

//builder.Host.UseNacosConfig(section: "NacosConfig");
AppSettings.InitAppSettings(builder.Configuration, builder.Environment.EnvironmentName);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
    })
    ;
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomService, CustomService>();

//builder.Services.AddMemoryCache();
//builder.Services.ConfigureDynamicProxy();

//Ö»ÆôÓÃÄÚ´æ»º´æ
//builder.Services.AddHeiHystrix();

//ÆôÓÃÄÚ´æ»º´æºÍredis»º´æ
//builder.Services.AddHeiHystrix(o =>
//{
//    o.RedisConnectionString = AppSettings.GetConnectionString("Redis");
//});

//ÆôÓÃÄÚ´æ»º´æºÍredis»º´æ£¬ÇÒÐÞ¸Ä»º´æÊý¾ÝµÄÐòÁÐ»¯ÅäÖÃ
builder.Services.AddHeiHystrix(o =>
{
    o.RedisConnectionString = AppSettings.GetConnectionString("Redis");
    o.JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
    {
        MaxDepth = 64,
        PropertyNameCaseInsensitive = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
});

builder.Host.UseServiceProviderFactory(new DynamicProxyServiceProviderFactory());

var app = builder.Build();
app.UseMetricServer();
app.UseHttpMetrics();
if (app.Environment.IsProduction() == false)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseDeveloperExceptionPage();
app.UseAuthorization();
app.MapControllers();
app.MapMetrics();
app.Run();