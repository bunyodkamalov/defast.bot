using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace Defast.Bot.Infrastructure.Common;

public class WarehousesService(
    IWarehousesRepository warehousesRepository,
    LoginSap loginSap,
    ICacheBroker cacheBroker,
    IOptions<RequestUrls> webSiteUris) : IWarehouseService
{
    public async ValueTask<List<Warehouse?>> GetWarehousesAsync(CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);
        
        var url = webSiteUris.Value.BaseUrl + webSiteUris.Value.GetWarehouses;

        return await warehousesRepository.Get(url, sessionId!, cancellationToken);
    }

    public async ValueTask<List<Warehouse?>> GetWarehouseInfoByWhsCodeAsync(string whsCode, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync("SessionKey", out string? sessionId, cancellationToken))
            sessionId = await loginSap.LoginSapAsync(cancellationToken);
        
        var url = webSiteUris.Value.BaseUrl + webSiteUris.Value.GetWarehouseInfo.Replace("{{whsCode}}", whsCode);

        return await warehousesRepository.Get(url, sessionId!, cancellationToken);
    }
}