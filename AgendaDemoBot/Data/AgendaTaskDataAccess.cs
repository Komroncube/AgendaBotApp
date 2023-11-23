using AgendaDemoBot.Models;
using MongoDB.Driver;
namespace AgendaDemoBot.Data
{
    public static class AgendaTaskDataAccess
    {
        private const string ConnectionString = "mongodb://localhost:27017";
        private const string DatabaseName = "TelegramBotAgendaDB";
        private const string UserCollection = "Users";

        private static IMongoCollection<T> ConnectToMongo<T>(in string collection)
        {
            var client = new MongoClient(ConnectionString);
            IMongoDatabase database = client.GetDatabase(DatabaseName);
            return database.GetCollection<T>(collection);

        }
        public static async ValueTask<ICollection<BotUser>> GetAllUsers()
        {
            IMongoCollection<BotUser> userCollection = ConnectToMongo<BotUser>(UserCollection);
            var users = await userCollection.FindAsync(_ => true);
            return await users.ToListAsync();
        }
        public static async ValueTask<BotUser> GetUserByIdAsync(long id)
        {
            IMongoCollection<BotUser> userCollection = ConnectToMongo<BotUser>(UserCollection);
            var filter = Builders<BotUser>.Filter.Eq(x => x.UserId, id);
            return await userCollection.Find(filter).FirstOrDefaultAsync();
        }

        public static Task UpdateUser(BotUser user)
        {
            IMongoCollection<BotUser> userCollection = ConnectToMongo<BotUser>(UserCollection);
            var filter = Builders<BotUser>.Filter.Eq(x => x.Id, user.Id);
            return userCollection.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
        }
        public static Task CreateUser(BotUser user)
        {
            IMongoCollection<BotUser> userCollection = ConnectToMongo<BotUser>(UserCollection);
            return userCollection.InsertOneAsync(user);
        }
    }
}
