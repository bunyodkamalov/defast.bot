using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Application.Common;

public interface IWarehouseService
{
    ValueTask<List<Warehouse?>> GetWarehousesAsync(CancellationToken cancellationToken);

    ValueTask<List<Warehouse?>> GetWarehouseInfoByWhsCodeAsync(string whsCode, CancellationToken cancellationToken);
}