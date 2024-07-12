using System.Text;
using Defast.Bot.Domain.Common.Caching;
using Defast.Bot.Domain.Entities.Identity;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Persistence.Caching.Brokers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Defast.Bot.Infrastructure.SAP;

public class LoginSap(
    ICacheBroker memoryCacheBroker,
    IOptions<SapUser> options,
    IOptions<RequestUrls> requestUris,
    IOptions<CacheSettings> cacheSettings
    )
{
    public async ValueTask<string> LoginSapAsync(CancellationToken cancellationToken)
    {
        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

        var url = requestUris.Value.BaseUrl + "Login";
        var payload = new SapUser()
        {
            CompanyDB = options.Value.CompanyDB,
            Password = options.Value.Password,
            UserName = options.Value.UserName
        };
        var payloadJson = JsonConvert.SerializeObject(payload);

        using (var client = new HttpClient(clientHandler))
        {
            var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var sessionKeyValue = JsonConvert.DeserializeObject<Session>(responseContent);
                await memoryCacheBroker.SetAsync("SessionKey", sessionKeyValue!.SessionId, new CacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(cacheSettings.Value.AbsoluteExpirationInMinutes), SlidingExpiration = TimeSpan.FromMinutes(cacheSettings.Value.SlidingExpirationInMinutes)});

                return sessionKeyValue.SessionId!;
            }
            else
                throw new InvalidOperationException("Wrong authorization details!");
        }
    }
}