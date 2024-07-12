using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Common.Caching;
using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Persistence.Caching.Brokers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirKunlik;

public class HandleOneDayCompletedOrders(IBusinessPartnerService businessPartnerService, IInvoicesService invoicesService, ICacheBroker cacheBroker)
{
    
    public async ValueTask<Message> Handle(int pageToken, ITelegramBotClient telegramBotClient, CallbackQuery callbackQuery, ELanguage eLanguage,
        CancellationToken cancellationToken)
    {
        if (!await cacheBroker.TryGetAsync<List<BusinessPartner?>>("users", out var users, cancellationToken))
        {
            users = await businessPartnerService.GetAsync(cancellationToken);
            await cacheBroker.SetAsync("users", users, cancellationToken: cancellationToken);
        }

        var businessPartner = users.First(bp => bp.U_TG_ID == callbackQuery.Message!.Chat.Id);

        var orders = await invoicesService.GetInvoicesByCardCodeAsync(businessPartner!.CardCode, cancellationToken);

        foreach (var invoice in orders.ToList())
            if (!(DateTime.Parse(invoice!.DocDueDate!) > DateTime.Now.AddDays(-1)))
                orders?.Remove(invoice);
        
        if (!orders!.Any())
            return await telegramBotClient.SendTextMessageAsync(
                callbackQuery.Message!.Chat.Id,
                eLanguage == ELanguage.Uzbek
                    ? "Bir kunlik tugallangan buyurtmalar topilmadi ❌"
                    : "Завершенные ордеров не найдено❌", 
                cancellationToken: cancellationToken);

        else
        {
            var messageText = eLanguage == ELanguage.Uzbek ? "Tugallangan haridlar ✅: \n\n" : "Завершенные ✅: \n\n";

            await cacheBroker.SetAsync("completedOrders", orders,
                new CacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1) }, cancellationToken);

            
            List<InlineKeyboardButton[]> inlineKeyboardButtons = new List<InlineKeyboardButton[]>();
            foreach (var order in orders!.Skip((pageToken - 1) * 10).Take(10))
                inlineKeyboardButtons.Add([
                    InlineKeyboardButton.WithCallbackData($"Hujjat raqami №{order.DocNum} | {DateTime.Parse(order.DocDueDate):dd.MM.yyyy}",
                        $"completedOrdersDocNum_{order.DocNum}")
                ]);

            InlineKeyboardButton[] inlineKeyBoardArray = new InlineKeyboardButton[orders!.Count / 10 + 1];

            for (int i = 1; i <= orders!.Count / 10 + 1; i++)
                inlineKeyBoardArray[i - 1] = InlineKeyboardButton.WithCallbackData($"{i}", $"completedOrdersOneDayPage_{i}");

            inlineKeyboardButtons.Add(inlineKeyBoardArray);
            InlineKeyboardMarkup inlineMarkup = new InlineKeyboardMarkup(inlineKeyboardButtons);

            await telegramBotClient.DeleteMessageAsync(callbackQuery.Message!.Chat.Id, callbackQuery.Message.MessageId, cancellationToken);
            
            return await telegramBotClient.SendTextMessageAsync(
                callbackQuery.Message!.Chat.Id,
                messageText,
                replyMarkup: inlineMarkup,
                cancellationToken: cancellationToken);
        }
        
    }
}