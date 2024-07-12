using Defast.Bot.Domain.Entities.Common;

namespace Defast.Bot.Application.Common;

public interface ITrackingService
{
    ValueTask<List<Tracking?>> GetAsync(CancellationToken cancellationToken);

    ValueTask<List<Tracking?>> GetByChatIdAsync(string chatId, CancellationToken cancellationToken);

    ValueTask<Tracking?> GetByDocNumAsync(string docNum, CancellationToken cancellationToken);
}