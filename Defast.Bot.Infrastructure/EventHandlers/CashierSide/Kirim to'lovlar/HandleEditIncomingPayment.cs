using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public class HandleEditIncomingPayment
{
    public async void Handle(Dictionary<ECurrency, decimal> ipAmounts, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();

        foreach (var currency in ipAmounts.Keys)
        {
            inlineKeyboardButtons.Add([
                InlineKeyboardButton.WithCallbackData($"{currency.ToString()}", $"editInPaymentAmount_{currency.ToString()}")
            ]);
        }

        inlineKeyboardButtons.Add([
            InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Haridni yakunlash ⏭️" : "Завершить покупку",
                "finishEditingIncomingPayments")
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