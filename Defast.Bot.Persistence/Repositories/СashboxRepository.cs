using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class СashboxRepository(ApplicationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<Сashbox, ApplicationDbContext>(dbContext, cacheBroker), IСashboxRepository
{
    public ValueTask<List<Сashbox?>> GetAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntitiesFromUrl(url, sessionId, cancellationToken);
    }
}