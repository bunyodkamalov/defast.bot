using System.Reflection;
using Defast.Bot.Application.Common;
using Defast.Bot.Domain.Entities.Identity;
using Defast.Bot.Domain.Settings;
using Defast.Bot.Infrastructure.Caching;
using Defast.Bot.Infrastructure.Common;
using Defast.Bot.Infrastructure.EventHandlers.Authorization;
using Defast.Bot.Infrastructure.EventHandlers.CashierSide.Kirim_to_lovlar;
using Defast.Bot.Infrastructure.EventHandlers.CashierSide.SendMoney;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TrackingSection;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirHaftalik;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirKunlik;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.BirOylik;
using Defast.Bot.Infrastructure.EventHandlers.Haridlar.TugallanganBuyurtmalar.HammaVaqtDavomida;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.AktSverka;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lov;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirHaftalik;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirKunlik;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.BirOylik;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.ChiqimTo_lovlarRo_yxati.HammaVaqt;
using Defast.Bot.Infrastructure.EventHandlers.To_lovlar.Qarzdorlik;
using Defast.Bot.Infrastructure.HostedServices;
using Defast.Bot.Infrastructure.SAP;
using Defast.Bot.Persistence.Caching.Brokers;
using Defast.Bot.Persistence.DataContexts;
using Defast.Bot.Persistence.Repositories;
using Defast.Bot.Persistence.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace Defast.Bot.Api.Configurations;

public static partial class HostConfigurations
{
    private static readonly ICollection<Assembly> Assemblies;

    static HostConfigurations()
    {
        Assemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(Assembly.Load).ToList();
        Assemblies.Add(Assembly.GetExecutingAssembly());
    }

    private static WebApplicationBuilder AddBusinessLogicInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.AddHostedService<ConfigureWebHook>();
        
        builder.Services.AddScoped<IEmployeesService, EmployeesService>();
        builder.Services.AddScoped<IBusinessPartnerService, BusinessPartnerService>();
        builder.Services.AddScoped<IWarehouseService, WarehousesService>();
        builder.Services.AddScoped<ICashboxService, CashboxService>();
        builder.Services.AddScoped<IIncomingPaymentsService, IncomingPaymentsService>();
        builder.Services.AddScoped<IInvoicesService, InvoicesService>();
        builder.Services.AddScoped<ITrackingService, TrackingService>();

        
        builder.Services.AddScoped<LoginSap>();

        builder.Services.AddScoped<HandleContactToLogin>();
        builder.Services.AddScoped<HandleCashierConfirmed>();
        builder.Services.AddScoped<HandleEditPayment>();
        builder.Services.AddScoped<HandleOutgoingPaymentConfirmed>();
        builder.Services.AddScoped<HandleOneWeekPeriod>();
        builder.Services.AddScoped<HandleOneWeekPeriodPagination>();
        builder.Services.AddScoped<HandleOneDayPeriod>();
        builder.Services.AddScoped<HandleOneDayPeriodPagination>();
        builder.Services.AddScoped<HandleOneMonthPeriod>();
        builder.Services.AddScoped<HandleOneMonthPeriodPagination>();
        builder.Services.AddScoped<HandleListOPayments>();
        builder.Services.AddScoped<HandleOutgoingPaymentsPagination>();
        builder.Services.AddScoped<HandleOutgoingPaymentDocNum>();
        builder.Services.AddScoped<HandleDebt>();

        builder.Services.AddScoped<HandleStart>();
        
        builder.Services.AddScoped<HandleCompletedOrdersAllPeriod>();
        builder.Services.AddScoped<HandleCompletedOrdersAllPeriodPagination>();
        builder.Services.AddScoped<HandleCompletedOrdersDocNum>();
        builder.Services.AddScoped<HandleCompletedOrdersOneDayPagination>();
        builder.Services.AddScoped<HandleCompletedOrdersOneWeekPagination>();
        builder.Services.AddScoped<HandleCompletedOrdersOneMonthPagination>();
        builder.Services.AddScoped<HandleOneWeekCompletedOrders>();
        builder.Services.AddScoped<HandleOneDayCompletedOrders>();
        builder.Services.AddScoped<HandleOneWeekCompletedOrdersOneMonth>();

        //Kirim to'lovlar
        builder.Services.AddScoped<HandleClientsPhone>();
        builder.Services.AddScoped<HandleEditIncomingPayment>();
        builder.Services.AddScoped<HandleIncomingPaymentConfirmed>();
        builder.Services.AddScoped<HandleAcceptIncomingPayment>();
        
        //SendMoney
        builder.Services.AddScoped<HandleIgnoreSendMoney>();
        builder.Services.AddScoped<HandleSendMoneyConfirmed>();

        //Tracking
        builder.Services.AddScoped<HandleTracking>();
        builder.Services.AddScoped<HandleTrackingPagination>();
        builder.Services.AddScoped<HandleTrackingsDocNum>();
        
        //AktSverka
        builder.Services.AddScoped<HandleAktSverka>();
        
        return builder;
    }

    private static WebApplicationBuilder AddMappers(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(Assemblies);
        return builder;
    }

    private static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddScoped<IEmployeesRepository, EmployeesRepository>();
        builder.Services.AddScoped<IBusinessPartnerRepository, BusinessPartnerRepository>();
        builder.Services.AddScoped<IWarehousesRepository, WarehousesRepository>();
        builder.Services.AddScoped<IСashboxRepository, СashboxRepository>();
        builder.Services.AddScoped<IBusinessPartnerUpdateRepository, BusinessPartnerUpdateRepository>();
        builder.Services.AddScoped<IIncomingPaymentRepository, IncomingPaymentRepository>();
        builder.Services.AddScoped<IInvoicesRepository, InvoicesRepository>();
        builder.Services.AddScoped<ITrackingRepository, TrackingRepository>();
        
        
        return builder;
    }

    private static WebApplicationBuilder AddCaching(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<CacheSettings>(builder.Configuration.GetSection(nameof(CacheSettings)));

        builder.Services.AddMemoryCache();

        builder.Services.AddSingleton<ICacheBroker, MemoryCacheBroker>();

        return builder;
    }

    private static WebApplicationBuilder AddExposers(this WebApplicationBuilder builder)
    {
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.Configure<RequestUrls>(builder.Configuration.GetSection(nameof(RequestUrls)));
        builder.Services.Configure<SapUser>(builder.Configuration.GetSection(nameof(SapUser)));
        builder.Services.Configure<HanaSqlUrls>(builder.Configuration.GetSection(nameof(HanaSqlUrls)));
        builder.Services.Configure<Chats>(builder.Configuration.GetSection(nameof(Chats)));
        
        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddHttpClient("tgwebhook")
            .AddTypedClient<ITelegramBotClient>(client
                => new TelegramBotClient(
                    builder.Configuration.GetSection("BotConfiguration").Get<BotConfigurations>()!.BotToken, client));


        builder.Services
            .AddControllers()
            .AddNewtonsoftJson();

        return builder;
    }

    private static WebApplicationBuilder AddDevTools(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }

    private static WebApplication UseExposers(this WebApplication app)
    {
        app.MapControllers();

        if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseCors();
        return app;
    }
}