using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati;

public static class HandleOutgoingPaymentPeriod
{
    public static async ValueTask Handle(ITelegramBotClient client, ELanguage eLanguage, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Bir kunlik" : "За один день",
                        "oneDayPeriod")
                ],
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Bir haftalik" : "За неделю",
                        "oneWeekPeriod")
                ],
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Bir oylik" : "За месяцу",
                        "oneMonthPeriod")
                ],
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        eLanguage == ELanguage.Uzbek ? "Hamma vaqt davomida" : "За все время", "allTimePeriod")
                }
            });

        await client.SendTextMessageAsync(callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "Vaqt kesimini tanlang" : "Выберите период",
            replyMarkup: inlineKeyboardMarkup, cancellationToken: cancellationToken);
    }
}