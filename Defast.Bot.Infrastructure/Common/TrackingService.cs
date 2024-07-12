using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class TrackingService(
    ITrackingRepository trackingRepository, 
    ICacheBroker cacheBroker, 
    LoginSap loginSap,
    IOptions<RequestUrls> requestUris) : ITrackingService
{

    public async ValueTask<List<Tracking?>> GetAsync(CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetTrackings;
        var result = await trackingRepository.GetAsync(url, sessionId!, cancellationToken);

        return result;   
    }

    public async ValueTask<List<Tracking?>> GetByChatIdAsync(string chatId, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetTrackingsByChatId.Replace("{{chatId}}", chatId);
        
        var result = await trackingRepository.GetAsync(url, sessionId!, cancellationToken);
        return result;
    }

    public async ValueTask<Tracking?> GetByDocNumAsync(string docNum, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetTrackingsByDocNum.Replace("{{docNum}}", docNum);
        
        var result = await trackingRepository.GetByDocNumAsync(url, sessionId!, cancellationToken);
        return result;    
    }
}