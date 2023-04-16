using System.Net;

namespace Server;

public class Client
{
    public Guid Id { get; init; }
    public EndPoint IpAddress { get; init; }
    public DateTime ConnectedAt { get; init; }

    public Client(EndPoint ipAddress)
    {
        Id = Guid.NewGuid();
        IpAddress = ipAddress;
        ConnectedAt = DateTime.UtcNow;
    }
}