using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public static class HandleEditSum
{
    public static async Task Handle(ITelegramBotClient botClient, CallbackQuery callback, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(callback.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "Kerakli summani yozing" : "Введите сумму",
            cancellationToken: cancellationToken);
    }
}