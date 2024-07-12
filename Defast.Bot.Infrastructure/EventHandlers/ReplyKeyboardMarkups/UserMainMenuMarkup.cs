using Defast.Bot.Domain.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;

public static class UserMainMenuMarkup
{
    public static ReplyKeyboardMarkup Get(ELanguage eLanguage)
    {
        var userMarkup = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton(eLanguage == ELanguage.Uzbek ? "To'lovlar" : "Платежи"),
                new KeyboardButton(eLanguage == ELanguage.Uzbek ? "Haridlar" : "Закупки")
            }
        ) { ResizeKeyboard = true, OneTimeKeyboard = true };
        return userMarkup;
    }
}