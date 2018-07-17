using MongoBannersContext.Documents;
using MongoDB.Driver;

namespace MongoBannersContext
{
    public class MongoBannersContext
    {
        private readonly IMongoDatabase _database = null;
        private readonly MongoClient _mongoClient;

        private const string BannersName = "Banners";

        public void ClearAll()
        {
            _database.GetCollection<BannerDoc>(BannersName).DeleteMany("{}");
        }

        public MongoBannersContext(string connectionString, string dbName)
        {
            _mongoClient = new MongoClient(connectionString);
            _database = _mongoClient.GetDatabase(dbName);
        }

        public IMongoCollection<BannerDoc> Banners
        {
            get
            {
                return _database.GetCollection<BannerDoc>(BannersName);
            }
        }
    }
}
