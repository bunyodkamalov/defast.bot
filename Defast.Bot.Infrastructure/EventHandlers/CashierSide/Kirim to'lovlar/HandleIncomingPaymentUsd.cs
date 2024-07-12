﻿using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public static class HandleIncomingPaymentUsd
{
    public static async ValueTask Handle(ITelegramBotClient botClient, CallbackQuery callbackQuery, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "USD da to'lov summasini kiriting 💲" : "Введите сумму оплаты в USD 💲", 
            cancellationToken: cancellationToken);
    }
}