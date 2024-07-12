using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public static class HandlePaymentComment
{
    public static async ValueTask Handle(ITelegramBotClient botClient, Message message, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            message.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "Izoh kiriting" : "Введите комментарий",
            cancellationToken: cancellationToken);
    }
}