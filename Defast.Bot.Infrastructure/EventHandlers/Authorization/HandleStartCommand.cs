using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Defast.Bot.Infrastructure.EventHandlers.Authorization;

public static class HandleStartCommand
{
    public static async ValueTask<Message> HandleAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;
        var messageText = message.Text;
        string firstname = message.From!.FirstName;
        
       
        var mainMenu = new InlineKeyboardMarkup(new[]
        {
            [InlineKeyboardButton.WithCallbackData(text: "O'zbek tili \ud83c\uddfa\ud83c\uddff", callbackData: "uzLang")],
            new[] { InlineKeyboardButton.WithCallbackData(text: "Русский язык \ud83c\uddf7\ud83c\uddfa", callbackData: "ruLang") }
        });
        
        Message sendMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text:
            $"Salom {firstname}! \nDavom etish uchun tilni tanlang\n\nЗдраствуйте {firstname}! \nВыберите язык, чтобы продолжить.",
            replyMarkup: mainMenu,
            cancellationToken: cancellationToken);

        return sendMessage;
    }
}