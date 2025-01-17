using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using WebApplication1.Models;

namespace WebApplication1.Service
{
    public class MongoDBservice
    {
        private readonly IMongoDatabase _database;
        public MongoDBservice(IConfiguration configuration)
        {
            var ConnectionString= configuration ["MongoDB:ConnectionString"];
            var databasename= configuration ["MongoDB:DatabaseName"];
            var client= new MongoClient(ConnectionString);
        _database = client.GetDatabase(databasename);
        }
       public IMongoCollection<User> Users =>_database.GetCollection<User>("Users");
    }
}