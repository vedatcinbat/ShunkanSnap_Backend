using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using UserService.Events;

namespace UserService.Services;

public class UserEventPublisher
{
    private readonly IConnection _connection;

    public UserEventPublisher()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
    }

    public void PublishUserCreated(User user)
    {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "user_created", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var userCreatedEvent = new UserCreatedEvent
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };

        var message = JsonConvert.SerializeObject(userCreatedEvent);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "user_created", basicProperties: null, body: body);
    }

    public void PublishUserDeleted(User user) {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "user_deleted", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var userDeletedEvent = new UserDeletedEvent
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            DeletedAt = user.UpdatedAt
        };

        var message = JsonConvert.SerializeObject(userDeletedEvent);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "user_deleted", basicProperties: null, body: body);
    }
    
    public void PublishUserSaved(User user) {
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "user_saved", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var userSavedEvent = new UserSavedEvent()
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            SavedAt = user.UpdatedAt
        };

        var message = JsonConvert.SerializeObject(userSavedEvent);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchange: "", routingKey: "user_saved", basicProperties: null, body: body);
    }
}