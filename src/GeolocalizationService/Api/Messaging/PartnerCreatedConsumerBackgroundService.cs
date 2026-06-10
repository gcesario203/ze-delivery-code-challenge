using System.Text;
using System.Text.Json;
using GeolocalizationService.Api.Grpc.Mappers;
using GeolocalizationService.Application.PartnerGeolocation.UseCases;
using GeolocalizationService.Application.Shared.Exceptions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ZeDelivery.Contracts.Messaging;

namespace GeolocalizationService.Api.Messaging;

public sealed class PartnerCreatedConsumerBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<PartnerCreatedConsumerBackgroundService> _logger;
    private IConnection _connection;
    private IChannel _channel;

    public PartnerCreatedConsumerBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<PartnerCreatedConsumerBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _settings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>() ?? new RabbitMqSettings();
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await EnsureConnectedAsync(stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, eventArgs) =>
        {
            try
            {
                var payload = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var message = JsonSerializer.Deserialize<PartnerCreatedIntegrationMessage>(payload)
                    ?? throw new InvalidOperationException("Invalid partner created message payload.");

                using var scope = _scopeFactory.CreateScope();
                var createPartnerGeolocation = scope.ServiceProvider.GetRequiredService<ICreatePartnerGeolocation>();

                await createPartnerGeolocation.ExecuteAsync(message.ToCreateInput());
                await _channel.BasicAckAsync(eventArgs.DeliveryTag, false, stoppingToken);

                _logger.LogInformation("Partner geolocation created for partner {PartnerId}", message.PartnerId);
            }
            catch (UseCaseValidationException validationException)
            {
                _logger.LogWarning(validationException, "Validation failed while consuming partner created message.");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, false, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to consume partner created message.");
                await _channel.BasicNackAsync(eventArgs.DeliveryTag, false, true, stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: MessagingConstants.PartnerGeolocationCreatedQueue,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password,
            VirtualHost = _settings.VirtualHost
        };

        _connection = await factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

        await _channel.ExchangeDeclareAsync(
            MessagingConstants.PartnerGeolocationCreatedExchange,
            ExchangeType.Topic,
            durable: true,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            MessagingConstants.PartnerGeolocationCreatedQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            MessagingConstants.PartnerGeolocationCreatedQueue,
            MessagingConstants.PartnerGeolocationCreatedExchange,
            MessagingConstants.PartnerGeolocationCreatedRoutingKey,
            cancellationToken: cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
