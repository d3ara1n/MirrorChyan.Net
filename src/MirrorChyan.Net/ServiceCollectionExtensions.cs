using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MirrorChyan.Net.Clients;
using MirrorChyan.Net.Services;
using Refit;

namespace MirrorChyan.Net;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMirrorChyan(
            string productId,
            string clientName,
            string currentVersion,
            Uri? baseAddress = null)
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            services
                .AddRefitClient<IMirrorChyanClient>(_ =>
                {
                    var settings = new RefitSettings(new SystemTextJsonContentSerializer(options));
                    settings.ExceptionFactory = async message => message switch
                    {
                        { IsSuccessStatusCode: true } => null,
                        {
                            StatusCode: HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized
                            or HttpStatusCode.Forbidden
                        } => null,
                        { RequestMessage: not null } => await ApiException
                            .Create(message.RequestMessage,
                                message.RequestMessage.Method,
                                message,
                                settings)
                            .ConfigureAwait(false),
                        _ => new NotImplementedException()
                    };

                    return settings;
                })
                .ConfigureHttpClient((sp, client) =>
                {
                    var o = sp.GetRequiredService<IOptionsMonitor<MirrorChyanOptions>>();
                    client.BaseAddress = o.CurrentValue.BaseAddress;
                });
            services
                .AddSingleton<IMirrorChyanService, MirrorChyanService>()
                .Configure<MirrorChyanOptions>(o =>
                {
                    o.ProductId = productId;
                    o.ClientName = clientName;
                    o.VersionString = currentVersion;
                    if (baseAddress is not null)
                        o.BaseAddress = baseAddress;
                });
            return services;
        }
    }
}