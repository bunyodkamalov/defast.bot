using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Domain.Settings;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;

public class HandleCashierConfirmed(
    IBusinessPartnerService businessPartnerService,
    IIncomingPaymentsService incomingPaymentsService,
    IOptions<Chats> chats)
{
    public async ValueTask Handle(Guid incomingPaymentId, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery, ELanguage language, CancellationToken cancellationToken)
    {
        var incomingPayment = await incomingPaymentsService.GetByIdAsync(incomingPaymentId, cancellationToken);
        
        if (incomingPayment is null)
        {
            await telegramBotClient.SendTextMessageAsync(chats.Value.CashierChatId,
                language == ELanguage.Uzbek ? "Bazada kirim to'lov topilmadi❌" : "Счета-фактуры не найдены❌",
                cancellationToken: cancellationToken);

            await telegramBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
                cancellationToken);
        }

        else
            await CreateIncomingPayment(telegramBotClient, language, incomingPayment, callbackQuery, cancellationToken);
    }

    private async ValueTask CreateIncomingPayment(ITelegramBotClient telegramBotClient,
        ELanguage language, IncomingPayment incomingPayment, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
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

        var businessPartners = await businessPartnerService.GetAsync(cancellationToken);
        var businessPartner = businessPartners.SingleOrDefault(bp => bp.CardCode == incomingPayment.CardCode);
        await telegramBotClient.SendTextMessageAsync(chats.Value.CashierChatId,
            language == ELanguage.Uzbek
                ? "Kirim to'lov yaratildi.✅\n" +
                  $"Summa: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                  $"Mijoz: {businessPartner!.CardName}\n"
               
                : "Входящиий платеж создан.✅\n" +
                  $"Сумма: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                  $"Клиент: {businessPartner!.CardName}",
            cancellationToken: cancellationToken);

        await telegramBotClient.SendTextMessageAsync(chats.Value.GroupChatId,
            language == ELanguage.Uzbek
                ? "Kirim to'lov yaratildi.✅\n" +
                  $"Summa: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                  $"Mijoz: {businessPartner!.CardName}\n"
                
                : "Входящиий платеж создан.✅" +
                  $"Сумма: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}\n" +
                  $"Клиент: {businessPartner!.CardName}",
            cancellationToken: cancellationToken);

        await telegramBotClient.SendTextMessageAsync(
            businessPartner.U_TG_ID!,
            language == ELanguage.Uzbek
                ? "Kirim to'lov tasdiqlandi.✅\n" +
                  $"Summa: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}" 
                  
                : "Входящиий платеж подтвержден.✅" +
                  $"Сумма: {incomingPayment.CashSum} {(incomingPayment.DocCurrency == ECurrency.USD.ToString() ? "$" : "so'm")}",
            cancellationToken: cancellationToken);

        await telegramBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
            cancellationToken);
        
        await incomingPaymentsService.DeleteByIdAsync(incomingPayment.Id, cancellationToken);
    }
}