using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Application.Common;

public interface IInvoicesService
{
    ValueTask<List<SalesInvoice?>> GetInvoicesByCardCodeAsync(string cardCode, CancellationToken cancellationToken);

    ValueTask<SalesInvoice?> GetByDocNumAsync(string docNum, CancellationToken cancellationToken);
}