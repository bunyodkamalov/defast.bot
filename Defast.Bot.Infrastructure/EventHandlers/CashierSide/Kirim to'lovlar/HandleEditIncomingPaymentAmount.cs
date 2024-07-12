using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public static class HandleEditIncomingPaymentAmount
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