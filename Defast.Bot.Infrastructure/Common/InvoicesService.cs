using AutoMapper;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class InvoicesService(
    ICacheBroker cacheBroker,
    LoginSap loginSap,
    IInvoicesRepository invoicesRepository,
    IMapper mapper,
    IOptions<RequestUrls> requestUris,
    IOptions<BotConfigurations> token) : IInvoicesService
{

    public async ValueTask<List<SalesInvoice?>> GetInvoicesByCardCodeAsync(string cardCode, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetInvoicesByCardCode
                        .Replace("{{cardCode}}", cardCode);

        return await invoicesRepository.GetByCardCode(url, sessionId!, cancellationToken);
    }

    public async ValueTask<SalesInvoice?> GetByDocNumAsync(string docNum, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetInvoiceByDocNum
                                        .Replace("{{docNum}}", docNum);

        return await invoicesRepository.GetByDocNum(url, sessionId!, cancellationToken);
    }
}