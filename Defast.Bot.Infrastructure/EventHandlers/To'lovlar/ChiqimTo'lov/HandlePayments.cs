using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public static class HandlePayments
{
    public static async void HandleAsync(ITelegramBotClient botClient, Message message,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [InlineKeyboardButton.WithCallbackData(text: eLanguage == ELanguage.Uzbek ? "Qarzdorlik" : "Долг", callbackData: "debt")],
                [InlineKeyboardButton.WithCallbackData(text: eLanguage == ELanguage.Uzbek ? "Chiqim to'lov yaratish" : "Добавит исходящие платежи", callbackData: "addOutgoingPayments")],
                [InlineKeyboardButton.WithCallbackData(text: eLanguage == ELanguage.Uzbek ? "Chiqim to'lovlar ro'yxati" : "Список исходящие платежи", callbackData: "listOutgoingPayments") ],
                new[] { InlineKeyboardButton.WithCallbackData("Акт сверка", callbackData: "aktSverka") },
            });

        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "Quyidagilardan birini tanlang" : "Выберите один из следующих",
            replyMarkup: inlineKeyboardMarkup,
            cancellationToken: cancellationToken
        );
    }
    
}