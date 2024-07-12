using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class CashboxService(
    IСashboxRepository cashBoxRepository,
    LoginSap loginSap, 
    ICacheBroker cacheBroker, 
    IOptions<RequestUrls> requestUrls
    ) : ICashboxService
{

    public async ValueTask<List<Сashbox?>> GetAsync(string accCode, string startDate, string endDate, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUrls.Value.BaseUrl + requestUrls.Value.GetCredits
            .Replace("{{accCode}}", accCode)
            .Replace("{{startDate}}", startDate)
            .Replace("{{endDate}}", endDate);

        return await cashBoxRepository.GetAsync(url, sessionId!, cancellationToken);
    }
}