using System.Globalization;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Persistence.Caching.Brokers;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;

public class HandleIgnoreSendMoney(
    ICacheBroker cacheBroker,
    IBusinessPartnerService businessPartnerService,
    IIncomingPaymentsService incomingPaymentsService,
    IOptions<Chats> chats)
{
    public async ValueTask Handle(Guid incomingPaymentId, ITelegramBotClient telegramBotClient,
        CallbackQuery callbackQuery, ELanguage eLanguage, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync<List<BusinessPartner?>>("users", out var users, cancellationToken))
        {
            users = await businessPartnerService.GetAsync(cancellationToken);
            await cacheBroker.SetAsync("users", users, cancellationToken: cancellationToken);
        }

        var incomingPayment = await incomingPaymentsService.GetByIdAsync(incomingPaymentId, cancellationToken);

        var businessPartner = users.First(user => user.CardCode == incomingPayment!.CardCode);

        await telegramBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId,
            cancellationToken);

        
        await telegramBotClient.SendTextMessageAsync(
            businessPartner!.U_TG_ID!,
            eLanguage == ELanguage.Uzbek
                ? "Pul ko'chirish rad etildi❌" +
                  $"Valyuta: {incomingPayment!.DocCurrency}\n" +
                  $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n"
                : "Перемешение денег отклонено❌" +
                  $"Валюта: {incomingPayment!.DocCurrency}\n" +
                  $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n",
            cancellationToken: cancellationToken);
        

        await telegramBotClient.SendTextMessageAsync(
            chats.Value.GroupChatId,
            eLanguage == ELanguage.Uzbek
                ? "Pul ko'chirish rad etildi❌" +
                  $"Kassir: {businessPartner.CardName}\n" +
                  $"Valyuta: {incomingPayment.DocCurrency}\n" +
                  $"Summa: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n"
                : "Перемешение денег отклонено❌" +
                  $"Кассир: {businessPartner.CardName}\n" +
                  $"Валюта: {incomingPayment.DocCurrency}\n" +
                  $"Сумма: {incomingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ')}\n",
            cancellationToken: cancellationToken);
        
        await incomingPaymentsService.DeleteByIdAsync(incomingPaymentId, cancellationToken);
    }
}