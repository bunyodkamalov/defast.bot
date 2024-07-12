using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public static class HandleSendMoneyComment
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