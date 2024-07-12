﻿using Defast.Bot.Domain.Enums;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public static class HandleSendMoneyUzs
{
    public static async ValueTask Handle(ITelegramBotClient botClient, CallbackQuery callbackQuery, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(
            callbackQuery.Message!.Chat.Id,
            eLanguage == ELanguage.Uzbek ? "UZS da to'lov summasini kiriting 💵" : "Введите сумму оплаты в UZS 💵", 
            cancellationToken: cancellationToken);
    }
}