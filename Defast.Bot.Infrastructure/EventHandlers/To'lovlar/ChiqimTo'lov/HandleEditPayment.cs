using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public class HandleEditPayment
{
    public async void Handle(Dictionary<ECurrency, decimal> goods, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();

        foreach (var currency in goods.Keys)
        {
            inlineKeyboardButtons.Add([
                InlineKeyboardButton.WithCallbackData($"{currency.ToString()}", $"editPaymentAmount_{currency.ToString()}")
            ]);
        }

        inlineKeyboardButtons.Add([
            InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Izoh" : "Коммент", "editComment")
        ]);

        inlineKeyboardButtons.Add([
            InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Haridni yakunlash ⏭️" : "Завершить покупку",
                "finishEditingPayments")
        ]);

        await telegramBotClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek
                ? "O'zgartirish uchun quyidagilardan birini tanlang \u2b07\ufe0f"
                : "Выберите одно из следующих, чтобы изменить \u2b07\ufe0f",
            replyMarkup: new InlineKeyboardMarkup(inlineKeyboardButtons),
            cancellationToken: cancellationToken);
    }
}