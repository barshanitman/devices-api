using System;
using System.Net;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.Extensions.Http;
using Polly;
using Swashbuckle.AspNetCore.SwaggerGen;
using Vnext.Function.Entities;

[assembly: FunctionsStartup(typeof(Vnext.Function.Startup))]
namespace Vnext.Function
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddTransient<IDeviceAsset, DeviceAsset>();
            builder.Services.AddHttpClient<IDeviceAsset, DeviceAsset>().AddTransientHttpErrorPolicy(
                policy => policy.OrResult(r => r.StatusCode == HttpStatusCode.BadRequest).WaitAndRetryAsync(10, _ => TimeSpan.FromSeconds(1))

            );
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING", EnvironmentVariableTarget.Process);

            builder.Services.AddDbContext<DeviceContext>(
            options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString));
            builder.Services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };






            // Then all subsequent manual serialization will be done with this setting.






        }

    }
}


