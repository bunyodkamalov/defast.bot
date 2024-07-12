using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Application.Common;

public interface ICashboxService
{
    ValueTask<List<Сashbox?>> GetAsync(string accCode, string startDate, string endDate, CancellationToken cancellationToken);
    
}