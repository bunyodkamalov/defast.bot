using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Application.Common;

public interface IIncomingPaymentsService
{
    ValueTask<IncomingPayment?> CreateAsync(IncomingPayment incomingPayment, CancellationToken cancellationToken);

    Task<List<IncomingPayment?>> GetByCardCodeAsync(string cardCode, CancellationToken cancellationToken);

    ValueTask<IncomingPayment?> InsertAsync(IncomingPayment incomingPayment, CancellationToken cancellationToken);

    ValueTask<IncomingPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    ValueTask<IncomingPayment?> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
}