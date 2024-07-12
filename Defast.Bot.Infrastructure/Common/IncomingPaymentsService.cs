using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class IncomingPaymentsService(
    IIncomingPaymentRepository incomingPaymentRepository,
    LoginSap loginSap,
    ICacheBroker cacheBroker,
    IOptions<RequestUrls> requestUris
    ) : IIncomingPaymentsService
{

    public async ValueTask<IncomingPayment?> CreateAsync(IncomingPayment incomingPayment, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);
        
        var url = requestUris.Value.BaseUrl + "IncomingPayments";
        
        return await incomingPaymentRepository.CreateAsync(incomingPayment, url, sessionId!, cancellationToken);
    }

    public async Task<List<IncomingPayment?>> GetByCardCodeAsync(string cardCode, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);

        var url = requestUris.Value.BaseUrl + requestUris.Value.GetIncomingPayments.Replace("{{cardCode}}", cardCode);
        
        return await incomingPaymentRepository.GetByCardCodeAsync(url, sessionId!, cancellationToken);
    }

    public async ValueTask<IncomingPayment?> InsertAsync(IncomingPayment incomingPayment, CancellationToken cancellationToken)
    {
        return await incomingPaymentRepository.InsertAsync(incomingPayment, cancellationToken);
    }

    public async ValueTask<IncomingPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await incomingPaymentRepository.GetByIdAsync(id, cancellationToken);
    }

    public async ValueTask<IncomingPayment?> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await incomingPaymentRepository.DeleteByIdAsync(id, cancellationToken);
    }
}