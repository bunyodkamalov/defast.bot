using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Authorization;

public static class HandleLanguage
{
    public static async void HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        var chatId = callbackQuery.Message!.Chat.Id;
        
        
        var getContactMarkup = new ReplyKeyboardMarkup(
            new[] { KeyboardButton.WithRequestContact(eLanguage == ELanguage.Uzbek ? "Raqam Yuborish" : "Отправить номер")}
        ) { ResizeKeyboard = true, OneTimeKeyboard = true };

        Message sendMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: eLanguage == ELanguage.Uzbek ? "Avtorizatsiya uchun telefon raqamingiz bilan ulashing.\ud83d\udcf1" : "Поделитесь контактом для авторизации\ud83d\udcf1",
            replyMarkup: getContactMarkup,
            cancellationToken: cancellationToken);
        
    }
}