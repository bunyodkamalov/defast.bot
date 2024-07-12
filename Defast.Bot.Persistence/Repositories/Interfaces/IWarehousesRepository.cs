using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IWarehousesRepository
{
    Task<List<Warehouse?>> Get(string url, string sessionId, CancellationToken cancellationToken);
}