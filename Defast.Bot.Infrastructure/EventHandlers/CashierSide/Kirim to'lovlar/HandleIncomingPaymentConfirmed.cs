using System.Globalization;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Domain.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;

public class HandleIncomingPaymentConfirmed(
    IIncomingPaymentsService incomingPaymentsService,
    IBusinessPartnerService businessPartnerService,
    IOptions<Chats> chats)
{
    public async ValueTask Handle(BusinessPartner businessPartner, Dictionary<ECurrency, decimal> ipAmounts,
        ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage language,
        CancellationToken cancellationToken)
    {
        foreach (var amount in ipAmounts)
        {
            var incomingPayment = new IncomingPayment()
            {
                CashAccount = amount.Key == ECurrency.USD ? "5030" : "5031",
                CardCode = businessPartner.CardCode,
                CardName = businessPartner.CardName,
                DocCurrency = amount.Key.ToString(),
                CashSum = amount.Value
            };
            
            var result = await incomingPaymentsService.CreateAsync(incomingPayment, cancellationToken);
            if (result is null)
            {
                await telegramBotClient.SendTextMessageAsync(chats.Value.CashierChatId,
                    language == ELanguage.Uzbek
                        ? "Kirim to'lov yaratishda xatolik yuz berdi.❌"
                        : "Произошла ошибка при создании платежа.❌",
                    cancellationToken: cancellationToken);
                return;
            }

            var createdIncomingPayment = incomingPaymentsService.GetByCardCodeAsync(result.CardCode, cancellationToken).Result.FirstOrDefault();

            if (createdIncomingPayment is not null)
            {
                await telegramBotClient.SendTextMessageAsync(chats.Value.CashierChatId,
                    language == ELanguage.Uzbek
                        ? "Kirim to'lov yaratildi.✅\n" +
                          $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                          $"Mijoz: {createdIncomingPayment.CardName}\n"

                        : "Входящиий платеж создан.✅\n" +
                          $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                          $"Клиент: {createdIncomingPayment.CardName}",
                    cancellationToken: cancellationToken);

                await telegramBotClient.SendTextMessageAsync(chats.Value.GroupChatId,
                    language == ELanguage.Uzbek
                        ? "Kirim to'lov yaratildi.✅\n" +
                          $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                          $"Mijoz: {createdIncomingPayment.CardName}\n"
                     
                        : "Входящиий платеж создан.✅" +
                          $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                          $"Клиент: {createdIncomingPayment.CardName}",
                    cancellationToken: cancellationToken);

                await telegramBotClient.SendTextMessageAsync(
                    (await businessPartnerService.GetAsync(cancellationToken)).First(bp => bp.CardCode == createdIncomingPayment.CardCode).U_TG_ID,
                    language == ELanguage.Uzbek
                        ? "Kirim to'lov tasdiqlandi.✅\n" +
                          $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}" 
                      
                        : "Входящиий платеж подтвержден.✅" +
                          $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}",
                    cancellationToken: cancellationToken);

                await telegramBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                    cancellationToken);
            }
        }
    }
}