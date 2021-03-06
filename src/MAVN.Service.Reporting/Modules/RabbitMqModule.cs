using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using MAVN.Service.Reporting.DomainServices.RabbitSubscribers;
using MAVN.Service.Reporting.Settings;
using Lykke.Service.CrossChainTransfers.Contract;
using Lykke.Service.PartnersPayments.Contract;
using Lykke.Service.PaymentTransfers.Contract;
using MAVN.Service.Reporting.Domain.Services;
using MAVN.Service.Reporting.DomainServices.EventHandlers;
using Lykke.Service.Staking.Contract.Events;
using Lykke.Service.Vouchers.Contract;
using Lykke.Service.WalletManagement.Contract.Events;
using Lykke.SettingsReader;

namespace MAVN.Service.Reporting.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        private const string DefaultQueueName = "reporting"; // named by my self

        private const string TransferToInternalCompletedExchangeName = "lykke.wallet.transfertointernalcompleted";
        private const string TransferToExternalProcessedExchangeName = "lykke.wallet.transfertoexternalprocessed";

        private const string TransferExchangeName = "lykke.wallet.transfer";
        private const string BonusReceivedExchangeName = "lykke.wallet.bonusreceived";
        private const string PartnersPaymentTokensReservedExchangeName = "lykke.wallet.partnerspaymenttokensreserved";
        private const string PartnersPaymentProcessedExchangeName = "lykke.wallet.partnerspaymentprocessed";
        private const string PaymentTransferTokensReservedExchangeName = "lykke.wallet.transfertokensreserved";
        private const string PaymentTransferProcessedExchangeName = "lykke.wallet.transferprocessed";
        private const string RefundPartnersPaymentExchangeName = "lykke.wallet.refundpartnerspayment";
        private const string RefundPaymentTransferExchangeName = "lykke.wallet.refundpaymenttransfer";
        private const string VoucherTokensReservedExchangeName = "lykke.wallet.vouchertokensreserved";
        private const string VoucherTokensUsedExchangeName = "lykke.wallet.vouchertokensused";
        private const string ReferralStakeReservedExchange = "lykke.wallet.referralstakereserved";
        private const string ReferralStakeReleasedExchange = "lykke.wallet.referralstakereleased";
        private const string ReferralStakeBurntExchange = "lykke.wallet.referralstakeburnt";

        private readonly string _connString;
        private readonly bool _isPublicBlockchainFeatureDisabled;

        public RabbitMqModule(IReloadingManager<AppSettings> settingsManager)
        {
            var appSettings = settingsManager.CurrentValue;
            _connString = appSettings.ReportService.Rabbit.Subscribers.ConnectionString;
            _isPublicBlockchainFeatureDisabled = appSettings.ReportService.IsPublicBlockchainFeatureDisabled
                ?? false;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them

            RegisterRabbitMqSubscribers(builder);
            RegisterSubscriberHandlers(builder);
        }

        private void RegisterSubscriberHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<P2PTransferHandler>()
                .As<IEventHandler<P2PTransferEvent>>()
                .SingleInstance();

            builder.RegisterType<BonusReceivedHandler>()
                .As<IEventHandler<BonusReceivedEvent>>()
                .SingleInstance();

            builder.RegisterType<PartnersPaymentTokensReservedHandler>()
                .As<IEventHandler<PartnersPaymentTokensReservedEvent>>()
                .SingleInstance();

            builder.RegisterType<PartnersPaymentProcessedHandler>()
                .As<IEventHandler<PartnersPaymentProcessedEvent>>()
                .SingleInstance();

            builder.RegisterType<PaymentTransferTokensReservedHandler>()
                .As<IEventHandler<PaymentTransferTokensReservedEvent>>()
                .SingleInstance();

            builder.RegisterType<PaymentTransferProcessedHandler>()
                .As<IEventHandler<PaymentTransferProcessedEvent>>()
                .SingleInstance();

            builder.RegisterType<ReferralStakeReleasedHandler>()
                .As<IEventHandler<ReferralStakeReleasedEvent>>()
                .SingleInstance();

            builder.RegisterType<ReferralStakeReservedHandler>()
                .As<IEventHandler<ReferralStakeReservedEvent>>()
                .SingleInstance();

            builder.RegisterType<ReferralStakeBurntHandler>()
                .As<IEventHandler<ReferralStakeBurntEvent>>()
                .SingleInstance();

            builder.RegisterType<RefundPartnersPaymentHandler>()
                .As<IEventHandler<RefundPartnersPaymentEvent>>()
                .SingleInstance();

            builder.RegisterType<RefundPaymentTransferHandler>()
                .As<IEventHandler<RefundPaymentTransferEvent>>()
                .SingleInstance();

            builder.RegisterType<TransferToExternalProcessedHandler>()
                .As<IEventHandler<TransferToExternalProcessedEvent>>()
                .SingleInstance();

            builder.RegisterType<TransferToInternalCompletedHandler>()
                .As<IEventHandler<TransferToInternalCompletedEvent>>()
                .SingleInstance();

            builder.RegisterType<VoucherTokensReservedHandler>()
                .As<IEventHandler<VoucherTokensReservedEvent>>()
                .SingleInstance();

            builder.RegisterType<VoucherTokensProcessedHandler>()
                .As<IEventHandler<VoucherTokensUsedEvent>>()
                .SingleInstance();
        }

        private void RegisterRabbitMqSubscribers(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitSubscriber<P2PTransferEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", TransferExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<BonusReceivedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", BonusReceivedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<PartnersPaymentTokensReservedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", PartnersPaymentTokensReservedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<PartnersPaymentProcessedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", PartnersPaymentProcessedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<PaymentTransferTokensReservedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", PaymentTransferTokensReservedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<PaymentTransferProcessedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", PaymentTransferProcessedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<RefundPartnersPaymentEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", RefundPartnersPaymentExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<RefundPaymentTransferEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", RefundPaymentTransferExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<ReferralStakeReleasedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", ReferralStakeReleasedExchange)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<ReferralStakeReservedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", ReferralStakeReservedExchange)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<ReferralStakeBurntEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", ReferralStakeBurntExchange)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<VoucherTokensReservedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", VoucherTokensReservedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<VoucherTokensUsedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", VoucherTokensUsedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            if (!_isPublicBlockchainFeatureDisabled)
                RegisterSubscribersForPublicBlockchainFeature(builder);
        }

        private void RegisterSubscribersForPublicBlockchainFeature(ContainerBuilder builder)
        {
            builder.RegisterType<RabbitSubscriber<TransferToExternalProcessedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", TransferToExternalProcessedExchangeName)
                .WithParameter("queueName", DefaultQueueName);

            builder.RegisterType<RabbitSubscriber<TransferToInternalCompletedEvent>>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _connString)
                .WithParameter("exchangeName", TransferToInternalCompletedExchangeName)
                .WithParameter("queueName", DefaultQueueName);
        }
    }
}

