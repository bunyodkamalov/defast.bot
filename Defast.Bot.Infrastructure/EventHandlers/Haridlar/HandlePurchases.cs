using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar;

public static class HandlePurchases
{
    public static async void HandleAsync(ITelegramBotClient botClient, Message message,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [InlineKeyboardButton.WithCallbackData(text: eLanguage == ELanguage.Uzbek ? "Tracking" : "Трекинг", callbackData: "tracking")],
                new[] { InlineKeyboardButton.WithCallbackData(text: eLanguage == ELanguage.Uzbek ? "Tugallangan haridlar" : "Завершение закупки", callbackData: "completed") },
            });

        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "Quyidagilardan birini tanlang" : "Выберите один из следующих",
            replyMarkup: inlineKeyboardMarkup,
            cancellationToken: cancellationToken
        );
    }
}