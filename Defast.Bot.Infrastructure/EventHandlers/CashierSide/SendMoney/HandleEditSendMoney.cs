using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public static class HandleEditSendMoney
{
    public static async void Handle(Dictionary<ECurrency, decimal> goods, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();

        foreach (var currency in goods.Keys)
        {
            inlineKeyboardButtons.Add([
                InlineKeyboardButton.WithCallbackData($"{currency.ToString()}", $"editSMAmount_{currency.ToString()}")
            ]);
        }

        inlineKeyboardButtons.Add([
            InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Izoh" : "Коммент", "editSMComment")
        ]);

        inlineKeyboardButtons.Add([
            InlineKeyboardButton.WithCallbackData(eLanguage == ELanguage.Uzbek ? "Haridni yakunlash ⏭️" : "Завершить покупку",
                "finishEditingSM")
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