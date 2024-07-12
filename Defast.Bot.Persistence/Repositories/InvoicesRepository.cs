using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class InvoicesRepository (
    ApplicationDbContext dbContext,
    ICacheBroker cacheBroker) : EntityRepositoryBase<SalesInvoice, ApplicationDbContext>(dbContext, cacheBroker), IInvoicesRepository
{
    
    public ValueTask<List<SalesInvoice?>> GetByCardCode(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntitiesFromUrl(url, sessionId, cancellationToken);
    }

    public ValueTask<SalesInvoice?> GetByDocNum(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntityFromUrl(url, sessionId, cancellationToken);
    }
}