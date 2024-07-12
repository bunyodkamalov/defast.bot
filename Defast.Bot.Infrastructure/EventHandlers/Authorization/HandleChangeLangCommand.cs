using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Authorization;

public static class HandleChangeLangCommand
{
    public async static ValueTask Handle(ITelegramBotClient telegramBotClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var messageText = message.Text;
        string firstname = message.From!.FirstName;

        
        var mainMenu = new InlineKeyboardMarkup(new[]
        {
            [InlineKeyboardButton.WithCallbackData(text: "O'zbek tili \ud83c\uddfa\ud83c\uddff", callbackData: "ChangeUzLang")],
            new[] { InlineKeyboardButton.WithCallbackData(text: "Русский язык \ud83c\uddf7\ud83c\uddfa", callbackData: "ChangeRuLang") }
        });
        
         await telegramBotClient.SendTextMessageAsync(
            chatId: chatId,
            text:
            $"O'zgartirish uchun tilni tanlang\n\nЗдраствуйте {firstname}! \nВыберите язык, чтобы изменить.",
            replyMarkup: mainMenu,
            cancellationToken: cancellationToken);
   
    }
}