using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IСashboxRepository
{
    ValueTask<List<Сashbox?>> GetAsync(string url, string sessionId, CancellationToken cancellationToken);
}