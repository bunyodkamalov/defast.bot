using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface ITrackingRepository
{
    ValueTask<List<Tracking?>> GetAsync(string url, string sessionId, CancellationToken cancellationToken);

    ValueTask<Tracking?> GetByDocNumAsync(string url, string sessionId, CancellationToken cancellationToken);

    ValueTask<Tracking?> UpdateAsync(Tracking tracking, string url, string sessionId, CancellationToken cancellationToken);
}