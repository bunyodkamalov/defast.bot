using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Persistence.Repositories.Interfaces;

public interface IIncomingPaymentRepository
{
    ValueTask<IncomingPayment?> CreateAsync(IncomingPayment incomingPayment, string url, string sessionId, CancellationToken cancellationToken);

    ValueTask<List<IncomingPayment?>> GetByCardCodeAsync(string url, string sessionId, CancellationToken cancellationToken);

    ValueTask<IncomingPayment?> InsertAsync(IncomingPayment incomingPayment, CancellationToken cancellationToken);

    ValueTask<IncomingPayment?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    ValueTask<IncomingPayment?> DeleteByIdAsync(Guid id, CancellationToken cancellationToken);
}