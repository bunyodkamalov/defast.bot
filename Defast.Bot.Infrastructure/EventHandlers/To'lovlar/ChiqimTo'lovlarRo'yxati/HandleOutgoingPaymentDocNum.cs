using System.Globalization;
using System.Text;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati;

public class HandleOutgoingPaymentDocNum(
    ICacheBroker cacheBroker,
    IBusinessPartnerService businessPartnerService,
    IIncomingPaymentsService incomingPaymentsService)
{
    public async ValueTask Handle(int docNum, ITelegramBotClient tgBotClient, Message message,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {

        var businessPartner = await businessPartnerService.GetByTgIdAsync(message.Chat.Id, cancellationToken);

        if (!await cacheBroker.TryGetAsync<List<IncomingPayment?>>("outgoingPayments", out var outgoningPayments, cancellationToken))
            outgoningPayments = await incomingPaymentsService.GetByCardCodeAsync(businessPartner.CardCode, cancellationToken);

        var outgoingPayment = outgoningPayments.FirstOrDefault(op => op.DocNum == docNum);

        if (outgoingPayment is not null)
        {
            var messageText = new StringBuilder(eLanguage == ELanguage.Uzbek
                ? "Chiqim to'lov: \n\n"
                : "Исходящие платеж: \n\n");

            messageText.Append(eLanguage == ELanguage.Uzbek
                ? $"\tHujjat raqami: {outgoingPayment.DocNum}\n" +
                  $"\t🗓️Sana: {DateTimeOffset.Parse(outgoingPayment.DocDate!):dd.MM.yyyy} \n" +
                  $"\tValyuta: {outgoingPayment.DocCurrency}\n" +
                  $"\tSumma : {(outgoingPayment.DocCurrency == ECurrency.USD.ToString() ? outgoingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ') : ((decimal)outgoingPayment.CashSumFC!).ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' '))}\n\n" +
                  $"Izoh: {outgoingPayment.Remarks}\n"

                : $"\tНомер документа: {outgoingPayment.DocNum}\n" +
                  $"\t🗓Дата: {DateTimeOffset.Parse(outgoingPayment.DocDate!):dd.MM.yyyy} \n" +
                  $"\tВалюта: {outgoingPayment.DocCurrency}\n" +
                  $"\tSumma : {(outgoingPayment.DocCurrency == ECurrency.USD.ToString() ? outgoingPayment.CashSum.ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' ') : ((decimal)outgoingPayment.CashSumFC!).ToString("#,##", CultureInfo.InvariantCulture).Replace(',', ' '))}\n\n" +
                  $"Izoh: {outgoingPayment.Remarks}");


            await tgBotClient.DeleteMessageAsync(message.Chat.Id, message.MessageId, cancellationToken);

            await tgBotClient.SendTextMessageAsync(
                message.Chat.Id,
                messageText.ToString(),
                replyMarkup: UserMainMenuMarkup.Get(eLanguage),
                cancellationToken: cancellationToken);
        }
    }
}