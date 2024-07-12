using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class IncomingPaymentRepository(
    ApplicationDbContext dbContext,
    ICacheBroker cacheBroker
    ) : EntityRepositoryBase<IncomingPayment, ApplicationDbContext>(dbContext, cacheBroker), IIncomingPaymentRepository
{
    public async ValueTask<IncomingPayment?> CreateAsync(IncomingPayment incomingPayment, string url, string sessionId, CancellationToken cancellationToken)
    {
        return await base.PostToSap(incomingPayment, url, sessionId, cancellationToken);
    }

    public async ValueTask<List<IncomingPayment?>> GetByCardCodeAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return await base.GetEntitiesFromUrl(url, sessionId, cancellationToken);
    }

    public async ValueTask<IncomingPayment?> InsertAsync(IncomingPayment incomingPayment, CancellationToken cancellationToken)
    {
        return await base.CreateAsync(incomingPayment, cancellationToken: cancellationToken);
    }

    public async ValueTask<IncomingPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await base.GetByIdAsync(id, cancellationToken: cancellationToken);
    }

    public async ValueTask<IncomingPayment?> DeleteByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await base.DeleteByIdAsync(id, cancellationToken: cancellationToken);
    }
}