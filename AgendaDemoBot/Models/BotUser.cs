using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgendaDemoBot.Models;

public class BotUser
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string UserName { get; set; }
    public long UserId { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<AgendaTask> Tasks { get; set; } = new List<AgendaTask>();

}
