using Microsoft.Extensions.Options;
using MirrorChyan.Net.Clients;
using MirrorChyan.Net.Services;

namespace MirrorChyan.Net;

public static class MirrorChyanServiceExtensions
{
    extension(IMirrorChyanService service)
    {
        public static IMirrorChyanService Create(Uri url, MirrorChyanOptions options,
            Func<HttpClient>? httpClientFactory = null)
        {
            var client = httpClientFactory is null
                ? new()
                : httpClientFactory();
            client.BaseAddress = url;
            return new MirrorChyanService(Refit.RestService.For<IMirrorChyanClient>(client),
                new OptionsWrapper<MirrorChyanOptions>(options));
        }
    }
}