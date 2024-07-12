using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories.Interfaces;

namespace Defast.Bot.Persistence.Repositories;

public class TrackingRepository(ApplicationDbContext dbContext, ICacheBroker cacheBroker)
    : EntityRepositoryBase<Tracking, ApplicationDbContext>(dbContext, cacheBroker), ITrackingRepository
{
    public ValueTask<List<Tracking?>> GetAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntitiesFromUrl(url, sessionId, cancellationToken);
    }

    public ValueTask<Tracking?> GetByDocNumAsync(string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.GetEntityFromUrl(url, sessionId, cancellationToken);
    }

    public ValueTask<Tracking?> UpdateAsync(Tracking tracking, string url, string sessionId, CancellationToken cancellationToken)
    {
        return base.UpdateFromSap(tracking, url, sessionId, cancellationToken);
    }
}