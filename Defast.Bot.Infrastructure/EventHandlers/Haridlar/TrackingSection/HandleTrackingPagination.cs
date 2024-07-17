using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar.TrackingSection;

public class HandleTrackingPagination(ICacheBroker cacheBroker, ITrackingService trackingService)
{
    public async ValueTask<Message> HandleAsync(int pageToken, ITelegramBotClient botClient, Message paginationMessage,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync<List<Tracking?>>("tracking", out var trackings, cancellationToken))
        {
            trackings = await trackingService.GetByChatIdAsync(paginationMessage.Chat.Id.ToString(), cancellationToken);
            await cacheBroker.SetAsync("outgoingPayments", trackings, cancellationToken: cancellationToken);
        }
        
        if(!trackings.Any())
            return await botClient.SendTextMessageAsync(
                paginationMessage.Chat.Id,
                eLanguage == ELanguage.Uzbek
                    ? "Chiqim to'lovlar topilmadi ❌"
                    : "Исходящие платежи не найдены❌",
                cancellationToken: cancellationToken);

        else
        {
            
            var messageText = eLanguage == ELanguage.Uzbek
                ? "Tracking \ud83d\udd04: \n\n"
                : "Трекинг \ud83d\udd04: \n\n";
            
            List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
            foreach (var tracking in trackings.Skip((pageToken - 1) * 10).Take(10))
                inlineKeyboardButtons.Add([
                    InlineKeyboardButton.WithCallbackData($"Hujjat raqami №{tracking!.DocNum} | {tracking.U_numberOfCntr}", $"trackingDocNum_{tracking.DocNum}")
                ]);

            InlineKeyboardButton[] inlineKeyBoardArray = new InlineKeyboardButton[trackings!.Count / 10 + 1];

            for (int i = 1; i <= trackings.Count / 10 + 1; i++)
                inlineKeyBoardArray[i - 1] = InlineKeyboardButton.WithCallbackData($"{i}", $"trackingPage_{i}");

            inlineKeyboardButtons.Add(inlineKeyBoardArray);
            InlineKeyboardMarkup inlineMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

            return await botClient.SendTextMessageAsync(
                paginationMessage.Chat.Id,
                messageText,
                replyMarkup: inlineMarkup,
                cancellationToken: cancellationToken);

        }
    }
}