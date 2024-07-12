using Defast.Bot.Domain.Entities.Common;
using Defast.Bot.Domain.Enums;
using Defast.Bot.Infrastructure.EventHandlers.Authorization;
using Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;
using Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TrackingSection;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirHaftalik;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirKunlik;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirOylik;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.HammaVaqtDavomida;
using Defast.Bot.Infrastructure.EventHandlers.ReplyKeyboardMarkups;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirHaftalik;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirKunlik;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirOylik;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.HammaVaqt;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.Qarzdorlik;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Defast.Bot.Infrastructure.HostedServices;

public class ConfigureWebHook : IHostedService
{
    private readonly IServiceScope _serviceScopeFactory;

    private Dictionary<long, CallbackQuery> _callbackQueries = new();

    private Dictionary<long, bool> _isRegistered = new();

    private Dictionary<ECurrency, decimal> _businessPartnerPayment = new();

    private Dictionary<ECurrency, decimal> _incomingPaymentAmounts = new();

    private Dictionary<ECurrency, decimal> _sendMoneyAmounts = new();
    
    private BusinessPartner _businessPartner;

    private Message _completedOrdersMessage;

    private Message trackingMessage;
    
    private string SendMoneyComment { get; set; }
    
    private Dictionary<long, string> PaymentComment { get; set; }


    private Message _outgoingPaymentsMessage;


    private long _chatId;

    private Dictionary<long, ELanguage> ELanguage { get; set; } = new();


    public ConfigureWebHook(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory.CreateScope();
    }

    public async Task StartAsync(CancellationToken stoppingToken)
    {
        string token = "7368436790:AAFMAVj1jpulhvmVujjSPwyBwcRDt5EosLk";
        var botClient = new TelegramBotClient(token);
        var cts = new CancellationTokenSource();

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };

        botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            stoppingToken);

        var me = await botClient.GetMeAsync(cancellationToken: stoppingToken);
        await botClient.DeleteWebhookAsync(cancellationToken: stoppingToken);

        Console.WriteLine($"Bot starting with {me.Username}");
        await Task.Delay(int.MaxValue, cts.Token);

        await cts.CancelAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Web hook removing (tugamoqda)");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Text)
            {
                var messageText = update.Message.Text;
                Console.WriteLine($"Xabar qabul qilindi \nChat id: {update.Message.Chat.Id}");
                _chatId = update.Message.Chat.Id;
                ELanguage.TryAdd(_chatId, Domain.Enums.ELanguage.Uzbek);


                switch (messageText)
                {
                    case "/start" when _isRegistered.TryGetValue(_chatId, out bool isRegistered) && isRegistered:
                        _chatId = update.Message.Chat.Id;
                        await client.SendTextMessageAsync(update.Message.Chat.Id,
                            ELanguage[_chatId] == Domain.Enums.ELanguage.Uzbek
                                ? "Quyidagilardan birini tanlang"
                                : "Выберите один из следующих",
                            replyMarkup: UserMainMenuMarkup.Get(ELanguage[_chatId]),
                            cancellationToken: cancellationToken);
                        break;

                    case "/start":
                        _chatId = update.Message.Chat.Id;
                        await HandleStartCommand.HandleAsync(client, update.Message, cancellationToken);
                        break;

                    case "/lang":
                        await HandleChangeLangCommand.Handle(client, update.Message, cancellationToken);
                        break;

                    case "To'lovlar" or "Платежи":
                        HandlePayments.HandleAsync(client, update.Message, ELanguage[update.Message.Chat.Id], cancellationToken);
                        break;

                    case "Haridlar" or "Закупки":
                        HandlePurchases.HandleAsync(client, update.Message, ELanguage[update.Message.Chat.Id], cancellationToken);
                        break;
                }

                if (_callbackQueries.Any())
                {
                    var callbackData = _callbackQueries[_chatId].Data;
                    switch (callbackData)
                    {
                        case "UZScomment":
                            if (!PaymentComment.TryAdd(_chatId, messageText!))
                                PaymentComment[_chatId] = messageText!;

                            await HandleConfirming.HandleAsync(_businessPartnerPayment, PaymentComment[_chatId], client, update.Message,
                                ELanguage[_chatId], cancellationToken);
                            break;
                        case "USDcomment":
                            if (!PaymentComment.TryAdd(_chatId, messageText!))
                                PaymentComment[_chatId] = messageText!;

                            await HandleConfirming.HandleAsync(_businessPartnerPayment, PaymentComment[_chatId], client, update.Message,
                                ELanguage[_chatId],
                                cancellationToken);
                            break;

                        case "UZS&USDcomment":
                            if (!PaymentComment.TryAdd(_chatId, messageText!))
                                PaymentComment[_chatId] = messageText!;

                            await HandleConfirming.HandleAsync(_businessPartnerPayment, PaymentComment[_chatId], client, update.Message,
                                ELanguage[_chatId],
                                cancellationToken);
                            break;

                        case "editComment":
                            if (!PaymentComment.TryAdd(_chatId, messageText!))
                                PaymentComment[_chatId] = messageText!;

                            var handleEditPayment1 = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleEditPayment>();
                            handleEditPayment1.Handle(_businessPartnerPayment, client, _callbackQueries[_chatId], ELanguage[_chatId],
                                cancellationToken);
                            break;

                        case "editOutgoingPayment":
                            var handleEditPayment2 = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleEditPayment>();
                            handleEditPayment2.Handle(_businessPartnerPayment, client, update.CallbackQuery!, ELanguage[_chatId], cancellationToken);
                            break;

                        case "UZSsendMoneyComment":
                            SendMoneyComment = messageText!;
                            await HandleConfirmingSendMoney.HandleAsync(_sendMoneyAmounts, SendMoneyComment, client, update.Message,
                                ELanguage[_chatId], cancellationToken);
                            break;

                        case "USDsendMoneyComment":
                            SendMoneyComment = messageText!;
                            await HandleConfirmingSendMoney.HandleAsync(_sendMoneyAmounts, SendMoneyComment, client, update.Message,
                                ELanguage[_chatId], cancellationToken);
                            break;
                            
                        case "UZS&USDsendMoneyComment":
                            SendMoneyComment = messageText!;
                            await HandleConfirmingSendMoney.HandleAsync(_sendMoneyAmounts, SendMoneyComment, client, update.Message,
                                ELanguage[_chatId], cancellationToken);
                            break;
                        
                        case "editSMComment":
                            SendMoneyComment = messageText!;
                            HandleEditSendMoney.Handle(_sendMoneyAmounts, client, _callbackQueries[_chatId], ELanguage[_chatId], cancellationToken);
                            break;
                    }


                    if (messageText!.StartsWith("+"))
                    {
                        var callbackDatas = _callbackQueries[_chatId].Data;
                        switch (callbackDatas)
                        {
                            case ("addIncomingPayment" or "incomingPaymentNotConfirmed" or "clientNotConfirmed"):
                                var handler = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleClientsPhone>();
                                _businessPartner = await handler.Handle(messageText, client, update.Message, ELanguage[update.Message.Chat.Id],
                                    cancellationToken);
                                break;
                        }
                    }

                    if (IsStringDigit(messageText) && _callbackQueries.Any())
                    {
                        switch (callbackData)
                        {
                            case { } text when text.StartsWith("editPaymentAmount_"):
                                var data = _callbackQueries[_chatId].Data!.Replace("editPaymentAmount_", "");
                                var currency = data == ECurrency.UZS.ToString()
                                    ? ECurrency.UZS
                                    : data == ECurrency.USD.ToString()
                                        ? ECurrency.USD
                                        : ECurrency.UZSUSD;

                                _businessPartnerPayment[currency] = decimal.Parse(messageText);
                                var handleEdit = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleEditPayment>();
                                handleEdit.Handle(_businessPartnerPayment, client, _callbackQueries[_chatId], ELanguage[_chatId],
                                    cancellationToken);
                                break;

                            case "UZS":
                                _businessPartnerPayment[ECurrency.USD] = decimal.Parse(messageText);
                                await HandlePaymentComment.Handle(client, update.Message!, ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "UZScomment";
                                break;

                            case "USD":
                                _businessPartnerPayment[ECurrency.USD] = decimal.Parse(messageText);
                                await HandlePaymentComment.Handle(client, update.Message!, ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "USDcomment";
                                break;

                            case "UZS&USD":
                                _businessPartnerPayment[ECurrency.UZS] = decimal.Parse(messageText);
                                await HandleUsd.Handle(client, _callbackQueries[_chatId], ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "USD";
                                break;

                            case "UZSincomingPayment":
                                _incomingPaymentAmounts[ECurrency.UZS] = decimal.Parse(messageText);
                                await HandleConfirmIncomingPayment.HandleAsync(_businessPartner, _incomingPaymentAmounts, client, update.Message,
                                    ELanguage[_chatId], cancellationToken);
                                break;

                            case "USDincomingPayment":
                                _incomingPaymentAmounts[ECurrency.USD] = decimal.Parse(messageText);
                                await HandleConfirmIncomingPayment.HandleAsync(_businessPartner, _incomingPaymentAmounts, client, update.Message,
                                    ELanguage[_chatId], cancellationToken);
                                break;

                            case "UZS&USDincomingPayment":
                                _incomingPaymentAmounts[ECurrency.UZS] = decimal.Parse(messageText);
                                await HandleUsd.Handle(client, _callbackQueries[_chatId], ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "USDincomingPayment";
                                break;

                            case { } cdata when cdata.StartsWith("editInPaymentAmount_"):
                                var callData = _callbackQueries[_chatId].Data!.Replace("editInPaymentAmount_", "");
                                var incomingPaymentCurrency = callData == ECurrency.UZS.ToString()
                                    ? ECurrency.UZS
                                    : callData == ECurrency.USD.ToString()
                                        ? ECurrency.USD
                                        : ECurrency.UZSUSD;

                                _incomingPaymentAmounts[incomingPaymentCurrency] = decimal.Parse(messageText);
                                var handleEditIncomingPayment = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleEditIncomingPayment>();
                                handleEditIncomingPayment.Handle(_incomingPaymentAmounts, client, _callbackQueries[_chatId], ELanguage[_chatId],
                                    cancellationToken);
                                break;
                            
                            case { } cdata when cdata.StartsWith("editSMAmount_"):
                                var sendMoneyData = _callbackQueries[_chatId].Data!.Replace("editSMAmount_", "");
                                var sendMoneyCurrency = sendMoneyData == ECurrency.UZS.ToString()
                                    ? ECurrency.UZS
                                    : sendMoneyData == ECurrency.USD.ToString()
                                        ? ECurrency.USD
                                        : ECurrency.UZSUSD;

                                _sendMoneyAmounts[sendMoneyCurrency] = decimal.Parse(messageText);
                                HandleEditSendMoney.Handle(_sendMoneyAmounts, client, _callbackQueries[_chatId], ELanguage[_chatId],
                                    cancellationToken);
                                break;
                                
                            case "UZSsendMoney":
                                _sendMoneyAmounts[ECurrency.UZS] = decimal.Parse(messageText);
                                await HandleSendMoneyComment.Handle(client, update.Message!, ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "UZSsendMoneyComment";
                                break;
                            
                            case "USDsendMoney":
                                _sendMoneyAmounts[ECurrency.USD] = decimal.Parse(messageText);
                                await HandleSendMoneyComment.Handle(client, update.Message!, ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "USDsendMoneyComment";
                                break;
                                
                            case "UZS&USDsendMoney":
                                _sendMoneyAmounts[ECurrency.UZS] = decimal.Parse(messageText);
                                await HandleSendMoneyUsd.Handle(client, _callbackQueries[_chatId], ELanguage[_chatId], cancellationToken);
                                _callbackQueries[_chatId].Data = "USDsendMoney";
                                break;

                        }
                    }
                }
            }


            if (update.CallbackQuery != null)
            {
                Console.WriteLine($"Callback query qabul qilindi {update.CallbackQuery.Data}");

                if (!_callbackQueries.Keys.Contains(_chatId))
                {
                    _chatId = update.CallbackQuery.Message!.Chat.Id;
                    _callbackQueries[_chatId] = update.CallbackQuery;
                }

                _callbackQueries[_chatId] = update.CallbackQuery;

                ELanguage.TryAdd(_chatId, Domain.Enums.ELanguage.Uzbek);

                switch (update.CallbackQuery.Data)
                {
                    case "uzLang":
                        ELanguage.TryAdd(_chatId, Domain.Enums.ELanguage.Uzbek);
                        HandleLanguage.HandleAsync(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "ruLang":
                        ELanguage[_chatId] = Domain.Enums.ELanguage.Russian;
                        HandleLanguage.HandleAsync(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "ChangeUzLang":
                        ELanguage[_chatId] = Domain.Enums.ELanguage.Uzbek;
                        await client.SendTextMessageAsync(update.CallbackQuery.Message!.Chat.Id,
                            ELanguage[_chatId] == Domain.Enums.ELanguage.Uzbek ? "Quyidagilardan birini tanlang" : "Выберите один из следующих",
                            replyMarkup: UserMainMenuMarkup.Get(ELanguage[_chatId]),
                            cancellationToken: cancellationToken);
                        break;

                    case "ChangeRuLang":
                        ELanguage[_chatId] = Domain.Enums.ELanguage.Russian;
                        await client.SendTextMessageAsync(update.CallbackQuery.Message!.Chat.Id,
                            ELanguage[_chatId] == Domain.Enums.ELanguage.Uzbek ? "Quyidagilardan birini tanlang" : "Выберите один из следующих",
                            replyMarkup: UserMainMenuMarkup.Get(ELanguage[_chatId]),
                            cancellationToken: cancellationToken);
                        break;

                    case "debt":
                        var handleDebt = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleDebt>();
                        await handleDebt.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "listOutgoingPayments":
                        await HandleOutgoingPaymentPeriod.Handle(client, ELanguage[_chatId], update.CallbackQuery,
                            cancellationToken);
                        break;


                    case "oneDayPeriod":
                        var handleOneDayPeriod = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneDayPeriod>();
                        _outgoingPaymentsMessage =
                            await handleOneDayPeriod.Handle(1, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("oneDayPeriodPage_"):
                        var oneDayPageToken = int.Parse(update.CallbackQuery.Data.Replace("oneDayPeriodPage_", ""));
                        var handleOneDayPeriodPagination = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneDayPeriodPagination>();
                        _outgoingPaymentsMessage = await handleOneDayPeriodPagination.Handle(oneDayPageToken, client, _outgoingPaymentsMessage,
                            ELanguage[_chatId], cancellationToken);
                        break;

                    case "oneWeekPeriod":
                        var handleOneWeekPeriod = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneWeekPeriod>();
                        _outgoingPaymentsMessage = await handleOneWeekPeriod.Handle(1, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("oneWeekPeriodPage_"):
                        var oneWeekPageToken = int.Parse(update.CallbackQuery.Data.Replace("oneWeekPeriodPage_", ""));
                        var handleOneWeekPeriodPagination = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneWeekPeriodPagination>();
                        _outgoingPaymentsMessage = await handleOneWeekPeriodPagination.Handle(oneWeekPageToken, client, _outgoingPaymentsMessage,
                            ELanguage[_chatId], cancellationToken);
                        break;

                    case "oneMonthPeriod":
                        var handleOneMonthPeriod = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneMonthPeriod>();
                        _outgoingPaymentsMessage =
                            await handleOneMonthPeriod.Handle(1, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("oneMonthPeriodPage_"):
                        var oneMonthPageToken = int.Parse(update.CallbackQuery.Data.Replace("oneMonthPeriodPage_", ""));
                        var handleOneMonthPeriodPagination =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneMonthPeriodPagination>();
                        _outgoingPaymentsMessage = await handleOneMonthPeriodPagination.Handle(oneMonthPageToken, client, _outgoingPaymentsMessage,
                            ELanguage[_chatId], cancellationToken);
                        break;

                    case "allTimePeriod":
                        var handleAllTimePeriod = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleListOPayments>();
                        _outgoingPaymentsMessage =
                            await handleAllTimePeriod.Handle(1, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("outgoingPaymentPage_"):
                        var outgoingPaymentPageToken = int.Parse(update.CallbackQuery.Data.Replace("outgoingPaymentPage_", ""));
                        var handleOutgoingPaymentsPagination =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOutgoingPaymentsPagination>();
                        _outgoingPaymentsMessage = await handleOutgoingPaymentsPagination.Handle(outgoingPaymentPageToken, client,
                            _outgoingPaymentsMessage, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("outgoingPaymentdocNum_"):
                        var docNum = int.Parse(update.CallbackQuery.Data.Replace("outgoingPaymentdocNum_", ""));
                        var handleOutgoingPaymentDocNum = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOutgoingPaymentDocNum>();
                        await handleOutgoingPaymentDocNum.Handle(docNum, client, _outgoingPaymentsMessage, ELanguage[_chatId], cancellationToken);
                        break;

                    case "addOutgoingPayments":
                        await HandleAddOutgoingPayments.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "outgoingPaymentNotConfirmed":
                        await HandleAddOutgoingPayments.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        _businessPartnerPayment.Clear();
                        break;

                    case { } data when data.StartsWith("cashierConfirmed_"):
                        var incomingPaymentId = Guid.Parse(update.CallbackQuery.Data.Replace("cashierConfirmed_", ""));
                        var handleCashierConfirmed = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCashierConfirmed>();
                        await handleCashierConfirmed.Handle(incomingPaymentId, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "finishEditingPayments":
                        await HandleConfirming.HandleAsync(_businessPartnerPayment, PaymentComment[_chatId], client, update.CallbackQuery.Message!,
                            ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("editPaymentAmount_"):
                        var currency = update.CallbackQuery.Data.Replace("editPaymentAmount_", "");
                        HandleEditPaymentAmount.Handle(currency, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("editSum_"):
                        await HandleEditSum.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "outgoingPaymentConfirmed":
                        var handleService = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOutgoingPaymentConfirmed>();
                        await handleService.Handle(_businessPartnerPayment, PaymentComment[_chatId], ELanguage[_chatId], client, update.CallbackQuery,
                            cancellationToken);
                        _businessPartnerPayment.Clear();
                        break;

                    case "editComment":
                        await HandlePaymentComment.Handle(client, update.CallbackQuery.Message!, ELanguage[_chatId], cancellationToken);
                        break;

                    case "USD":
                        await HandleUsd.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "UZS":
                        await HandleUzs.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "UZS&USD":
                        await HandleUzs.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "completed":
                        await HandleCompletedOrdersPeriod.Handle(client, ELanguage[_chatId], update.CallbackQuery!, cancellationToken);
                        break;

                    case "completedAllPeriod":
                        var confirmedPurchases = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCompletedOrdersAllPeriod>();
                        _completedOrdersMessage =
                            await confirmedPurchases.Handle(1, client, update.CallbackQuery!, ELanguage[_chatId], cancellationToken);
                        break;

                    case "completedOneDay":
                        var confirmedPurchasesOneDay = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneDayCompletedOrders>();
                        _completedOrdersMessage =
                            await confirmedPurchasesOneDay.Handle(1, client, update.CallbackQuery!, ELanguage[_chatId], cancellationToken);
                        break;

                    case "completedOneMonth":
                        var confirmedPurchasesOneMonth =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneWeekCompletedOrdersOneMonth>();
                        _completedOrdersMessage =
                            await confirmedPurchasesOneMonth.Handle(1, client, update.CallbackQuery!, ELanguage[_chatId], cancellationToken);
                        break;

                    case "completedOneWeek":
                        var confirmedPurchasesOneWeek = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleOneWeekCompletedOrders>();
                        _completedOrdersMessage =
                            await confirmedPurchasesOneWeek.Handle(1, client, update.CallbackQuery!, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("completedOrdersPage_"):
                        var pageToken = int.Parse(update.CallbackQuery!.Data!.Replace("completedOrdersPage_", ""));
                        var handleCompletedOrdersPagination =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCompletedOrdersAllPeriodPagination>();
                        _completedOrdersMessage = await handleCompletedOrdersPagination.Handle(pageToken, client, _completedOrdersMessage,
                            ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("completedOrdersOneWeekPage_"):
                        var pageTokenOneWeek = int.Parse(update.CallbackQuery!.Data!.Replace("completedOrdersPage_", ""));
                        var handleCompletedOrdersPaginationOneWeek =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCompletedOrdersOneWeekPagination>();
                        _completedOrdersMessage = await handleCompletedOrdersPaginationOneWeek.Handle(pageTokenOneWeek, client,
                            _completedOrdersMessage, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("completedOrdersOneDayPage_"):
                        var pageTokenOneDay = int.Parse(update.CallbackQuery!.Data!.Replace("completedOrdersPage_", ""));
                        var handleCompletedOrdersPaginationOneDay =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCompletedOrdersOneDayPagination>();
                        _completedOrdersMessage = await handleCompletedOrdersPaginationOneDay.Handle(pageTokenOneDay, client, _completedOrdersMessage,
                            ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("completedOrdersOneMonthPage_"):
                        var pageTokenOneMonth = int.Parse(update.CallbackQuery!.Data!.Replace("completedOrdersPage_", ""));
                        var handleCompletedOrdersPaginationOneMoth =
                            _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCompletedOrdersOneMonthPagination>();
                        _completedOrdersMessage = await handleCompletedOrdersPaginationOneMoth.Handle(pageTokenOneMonth, client,
                            _completedOrdersMessage, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("completedOrdersDocNum_"):
                        var completedOrdersdocNum = int.Parse(update.CallbackQuery!.Data!.Replace("completedOrdersDocNum_", ""));
                        var handleCompletedOrdersDocNum = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleCompletedOrdersDocNum>();
                        await handleCompletedOrdersDocNum.Handle(completedOrdersdocNum, client, update.CallbackQuery, ELanguage[_chatId],
                            cancellationToken);
                        break;

                    case "addIncomingPayment":
                        await HandleAddIncomingPayment.HandleAsync(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "clientNotConfirmed":
                        await HandleAddIncomingPayment.HandleAsync(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "incomingPaymentNotConfirmed":
                        await HandleAddIncomingPayment.HandleAsync(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        _incomingPaymentAmounts.Clear();
                        break;

                    case "clientConfirmed":
                        await HandleClientConfirmed.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "finishEditingIncomingPayments":
                        await HandleConfirmIncomingPayment.HandleAsync(_businessPartner, _incomingPaymentAmounts, client,
                            update.CallbackQuery.Message!, ELanguage[_chatId], cancellationToken);
                        break;

                    case "editIncomingPayment":
                        var handleEditInPayment = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleEditIncomingPayment>();
                        handleEditInPayment.Handle(_incomingPaymentAmounts, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("editInPaymentAmount_"):
                        var inPaymentCurrency = update.CallbackQuery.Data.Replace("editInPaymentAmount_", "");
                        HandleEditIncomingPaymentAmount.Handle(inPaymentCurrency, client, update.CallbackQuery, ELanguage[_chatId],
                            cancellationToken);
                        break;

                    case "incomingPaymentConfirmed":
                        var ipConfirmed = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleIncomingPaymentConfirmed>();
                        await ipConfirmed.Handle(_businessPartner, _incomingPaymentAmounts, client, update.CallbackQuery, ELanguage[_chatId],
                            cancellationToken);
                        _incomingPaymentAmounts.Clear();
                        _businessPartner = default;
                        break;

                    case "UZSincomingPayment":
                        await HandleIncomingPaymentUzs.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "USDincomingPayment":
                        await HandleIncomingPaymentUsd.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case "UZS&USDincomingPayment":
                        await HandleIncomingPaymentUzs.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;

                    case { } data when data.StartsWith("acceptIncomingPayment_"):
                        var sendMoneyIncomingPaymentId = Guid.Parse(update.CallbackQuery.Data.Replace("acceptIncomingPayment_", ""));
                        var acceptIncomingPayment = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleAcceptIncomingPayment>();
                        await acceptIncomingPayment.Handle(sendMoneyIncomingPaymentId, client, update.CallbackQuery, ELanguage[_chatId],
                            cancellationToken);
                        break;
                    
                    case "finishEditingSM":
                        await HandleConfirmingSendMoney.HandleAsync(_sendMoneyAmounts, SendMoneyComment, client,
                            update.CallbackQuery.Message!, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case "editSendMoney":
                        HandleEditSendMoney.Handle(_sendMoneyAmounts, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case { } data when data.StartsWith("editSMAmount_"):
                        var smCurrency = update.CallbackQuery.Data.Replace("editSMAmount_", "");
                        HandleEditSendMoneyAmount.Handle(smCurrency, client, update.CallbackQuery, ELanguage[_chatId],
                            cancellationToken);
                        break;
                    
                    case { } data when data.StartsWith("ignoreIncomingPayment_"):
                        var smIncomingPaymentId = Guid.Parse(update.CallbackQuery.Data.Replace("ignoreIncomingPayment_", ""));
                        var handleIgnoreSendMoney = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleIgnoreSendMoney>();
                        await handleIgnoreSendMoney.Handle(smIncomingPaymentId, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case "sendMoney":
                        await HandleSendMoney.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case "sendMoneyNotConfirmed":
                        await HandleSendMoney.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        _sendMoneyAmounts.Clear();
                        break;
                        
                    case "editSMComment":
                        await HandleSendMoneyComment.Handle(client, update.CallbackQuery!.Message!, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case "sendMoneyConfirmed":
                        var handler = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleSendMoneyConfirmed>();
                        await handler.Handle(_sendMoneyAmounts, SendMoneyComment, ELanguage[_chatId], client, update.CallbackQuery, cancellationToken);
                        _sendMoneyAmounts.Clear();
                        SendMoneyComment = "";
                        break;
                    
                    case "UZSsendMoney":
                        await HandleSendMoneyUzs.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                        
                    case "USDsendMoney":
                        await HandleSendMoneyUsd.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                        
                    case "UZS&USDsendMoney":
                        await HandleSendMoneyUzs.Handle(client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case "tracking":
                        var handleTracking = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleTracking>();
                        trackingMessage = await handleTracking.HandleAsync(1, client, update.CallbackQuery, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case { } data when data.StartsWith("trackingPage_"):
                        var trackingPageToken = int.Parse(data.Replace("trackingPage_", ""));
                        var handleTrackingPage = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleTrackingPagination>();
                        await handleTrackingPage.HandleAsync(trackingPageToken, client, trackingMessage, ELanguage[_chatId], cancellationToken);
                        break;
                    
                    case { } data when data.StartsWith("trackingDocNum_"):
                        var trackingDocNum = int.Parse(data.Replace("trackingDocNum_", ""));
                        var handleTrackingDocNum = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleTrackingsDocNum>();
                        await handleTrackingDocNum.Handle(trackingDocNum, client, trackingMessage, ELanguage[_chatId], cancellationToken);
                        break;
                    
                }
            }

            if (update.Type == UpdateType.Message && update.Message!.Type == MessageType.Contact)
            {
                var contactHandler = _serviceScopeFactory.ServiceProvider.GetRequiredService<HandleContactToLogin>();
                var businessPartner = await contactHandler.HandleAsync(client, update.Message, ELanguage[_chatId], cancellationToken);

                if (businessPartner is not null)
                    _isRegistered.Add(_chatId, true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception {e.Message} \n {e.StackTrace}");
            foreach (var exception in e.Data)
            {
                Console.WriteLine(exception.ToString());
            }
        }
    }

    Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram Api error: \n {apiRequestException.ErrorCode} \n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    static bool IsStringDigit(string str)
    {
        foreach (char c in str)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }
}