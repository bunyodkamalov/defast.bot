using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class WarehousesRepository(ApplicationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<Warehouse, ApplicationDbContext>(dbContext, cacheBroker), IWarehousesRepository
{
    public async Task<List<Warehouse?>> Get(string url, string sessionId, CancellationToken cancellationToken)
    {
        return await base.GetEntitiesFromUrl(url, sessionId, cancellationToken);
    }
}