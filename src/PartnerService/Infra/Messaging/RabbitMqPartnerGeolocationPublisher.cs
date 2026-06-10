using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PartnerService.Application.GeoLocalization.Events;
using RabbitMQ.Client;
using ZeDelivery.Contracts.Messaging;

namespace PartnerService.Infra.Messaging;

public interface IPartnerGeolocationPublisher
{
    Task PublishPartnerCreatedAsync(PartnerCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}

public sealed class RabbitMqPartnerGeolocationPublisher : IPartnerGeolocationPublisher, IAsyncDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqPartnerGeolocationPublisher> _logger;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private IConnection _connection;
    private IChannel _channel;

    public RabbitMqPartnerGeolocationPublisher(
        IConfiguration configuration,
        ILogger<RabbitMqPartnerGeolocationPublisher> logger)
    {
        _settings = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>() ?? new RabbitMqSettings();
        _logger = logger;
    }

    public async Task PublishPartnerCreatedAsync(
        PartnerCreatedIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        await EnsureConnectedAsync(cancellationToken);

        var message = new PartnerCreatedIntegrationMessage
        {
            PartnerId = integrationEvent.PartnerId,
            Address = new AddressMessage
            {
                Cordinates = new CoordinateMessage
                {
                    Latitude = integrationEvent.Address.Cordinates.Latitude,
                    Longitude = integrationEvent.Address.Cordinates.Longitude
                }
            },
            CoverageArea = new CoverageAreaMessage
            {
                Cordinates = integrationEvent.CoverageArea.Cordinates
                    .Select(polygon => polygon
                        .Select(ring => ring
                            .Select(coordinate => new CoordinateMessage
                            {
                                Latitude = coordinate.Latitude,
                                Longitude = coordinate.Longitude
                            })
                            .ToList())
                        .ToList())
                    .ToList()
            }
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        var properties = new BasicProperties { Persistent = true };

        await _channel.BasicPublishAsync(
            exchange: MessagingConstants.PartnerGeolocationCreatedExchange,
            routingKey: MessagingConstants.PartnerGeolocationCreatedRoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published partner geolocation created message for partner {PartnerId}", integrationEvent.PartnerId);
    }

    private async Task EnsureConnectedAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
            return;

        await _connectionLock.WaitAsync(cancellationToken);

        try
        {
            if (_channel is not null)
                return;

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
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
            await _channel.DisposeAsync();

        if (_connection is not null)
            await _connection.DisposeAsync();

        _connectionLock.Dispose();
    }
}
