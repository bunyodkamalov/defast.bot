using System.Text;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar;

public class HandleCompletedOrdersDocNum(IBusinessPartnerService businessPartnerService, IInvoicesService invoicesService, ICacheBroker cacheBroker)
{
    public async ValueTask Handle(int docNum, ITelegramBotClient tgBotClient, CallbackQuery callbackQuery,
        ELanguage eLanguage, CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync<List<BusinessPartner?>>("users", out var users, cancellationToken))
        {
            users = await businessPartnerService.GetAsync(cancellationToken);
            await cacheBroker.SetAsync("users", users, cancellationToken: cancellationToken);
        }

        var businessPartner = users.FirstOrDefault(bp => bp.U_TG_ID == callbackQuery.Message!.Chat.Id);

        var orders = await invoicesService.GetInvoicesByCardCodeAsync(businessPartner!.CardCode, cancellationToken);

        var invoice = orders!.SingleOrDefault(quot => quot.DocNum == docNum);
        
        if (invoice is not null)
        {
            var messageText = new StringBuilder(eLanguage == ELanguage.Uzbek
                ? "Yakunlangan buyurtmalar ✅: \n\n"
                : "Выполненные заказы ✅: \n\n");

            messageText.Append(eLanguage == ELanguage.Uzbek 
                ? $"Hujjat raqami № {invoice.DocNum}\n" +
                  $"Konteyner raqami: {invoice.U_numberOfCntr} \n" + 
                  $"🗓️Sana: {DateTimeOffset.Parse(invoice.DocDueDate!):dd.MM.yyyy} \n\n" 
                : $"Номер документа № {invoice.DocNum}\n\n" +
                  $"🗓Дата: {DateTimeOffset.Parse(invoice.DocDueDate!):dd.MM.yyyy} \n\n");
            decimal overAllQuantity = 0;
            
            foreach (var item in invoice.DocumentLines)
            {
                overAllQuantity += (decimal)item.InventoryQuantity!;
                
                messageText.Append(eLanguage == ELanguage.Uzbek
                    ? $"\ud83d\udce6 Tovar:  {item.ItemDescription}\n"
                    : $"\t\ud83d\udce6 Товар: {item.ItemDescription}\n" );
            }

            messageText.Append(
                eLanguage == ELanguage.Uzbek 
                    ? $"\ud83d\udcac Izoh :  {invoice.Comments}\n\n" +
                      $"💲Jami Summa: {invoice.DocTotal} $"
                      
                    : $"\ud83d\udcac Комментарий:  {invoice.Comments}\n\n" +
                      $"💲Общая сумма: {invoice.DocTotal} $");
            
            await tgBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, cancellationToken);

            await tgBotClient.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                messageText.ToString(),
                replyMarkup: UserMainMenuMarkup.Get(eLanguage),
                cancellationToken: cancellationToken);
        }
    }
    
}