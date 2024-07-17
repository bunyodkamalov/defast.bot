using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.AktSverka;

public static class HandlePeriods
{
    public static async ValueTask HandleAsync(ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage language, CancellationToken cancellationToken)
    {
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData(language == ELanguage.Uzbek ? "Bir kunlik" : "За неделю",
                        $"aktSverkaOneDayPeriod")
                ],
                [
                    InlineKeyboardButton.WithCallbackData(language == ELanguage.Uzbek ? "Bir haftalik" : "За неделю",
                        $"aktSverkaOneWeekPeriod")
                ],
                [
                    InlineKeyboardButton.WithCallbackData(language == ELanguage.Uzbek ? "Bir oylik" : "За месяцу",
                        $"aktSverkaOneMonthPeriod")
                ],
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        language == ELanguage.Uzbek ? "Hamma vaqt davomida" : "За все время", $"aktSverkaAllTimePeriod")
                }
            });

        await telegramBotClient.SendTextMessageAsync(callbackQuery.Message!.Chat.Id,
            language == ELanguage.Uzbek ? "Akt sverkani ko'rish uchun vaqt kesimini tanlang" : "Выберите период для просмотра акт сверки",
            replyMarkup: inlineKeyboardMarkup, cancellationToken: cancellationToken);
    }
}