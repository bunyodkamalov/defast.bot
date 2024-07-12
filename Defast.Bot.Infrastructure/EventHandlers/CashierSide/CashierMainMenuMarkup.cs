using Defast.Bot.Domain.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide;

public static class CashierMainMenuMarkup
{
    public static InlineKeyboardMarkup Get(ELanguage eLanguage)
    {
        var userMarkup = new InlineKeyboardMarkup(
            new[]
            {
                [
                    InlineKeyboardButton.WithCallbackData(
                        eLanguage == ELanguage.Uzbek ? "Kirim to'lovlar" : "Входящие платежи", "addIncomingPayment")],
                new[]{
                    InlineKeyboardButton.WithCallbackData(
                        eLanguage == ELanguage.Uzbek ? "Pul ko'chirish" : "Перемешение денег", "sendMoney")
                }
            }
        );
        return userMarkup;
    }
}