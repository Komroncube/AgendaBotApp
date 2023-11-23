using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgendaDemoBot.Models
{
    public class AgendaTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TaskDefinition { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastSendDate { get; set; }
    }
}
