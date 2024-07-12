using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public static class HandleEditPaymentAmount
{
    public static async void Handle(string currency, ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        
        await telegramBotClient.SendTextMessageAsync(callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek
                ? $"{currency} ni summasini yozing:"
                : $"Введите сумму в {currency}",
            cancellationToken: cancellationToken);
    } 
}