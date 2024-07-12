using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IInvoicesRepository
{
    ValueTask<List<SalesInvoice?>> GetByCardCode(string url, string sessionId, CancellationToken cancellationToken);

    ValueTask<SalesInvoice?> GetByDocNum(string url, string sessionId, CancellationToken cancellationToken);
}