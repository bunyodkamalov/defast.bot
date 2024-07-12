using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar;

public static class HandleCompletedOrdersPeriod
{
    public static async ValueTask Handle(ITelegramBotClient client, ELanguage eLanguage, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Bir kunlik" : "За один день",
                        "completedOneDay")
                ],
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Bir haftalik" : "За неделю",
                        "completedOneWeek")
                ],
                [
                    InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Bir oylik" : "За месяцу",
                        "completedOneMonth")
                ],
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        eLanguage == ELanguage.Uzbek ? "Hamma vaqt davomida" : "За все время", "completedAllPeriod")
                }
            });

        await client.SendTextMessageAsync(callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "Vaqt kesimini tanlang" : "Выберите период",
            replyMarkup: inlineKeyboardMarkup, cancellationToken: cancellationToken);
    }
}