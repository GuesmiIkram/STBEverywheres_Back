using Microsoft.Extensions.Options;
using MongoDB.Driver;
using STBEverywhere_ApiAgence.Models;
using System.Text.Json;

namespace STBEverywhere_ApiAgence.Services
{
    public class AgenceService
    {
        private readonly IMongoCollection<Agence> _agences;

        public AgenceService(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _agences = database.GetCollection<Agence>(settings.Value.CollectionName);
        }

        public async Task InitializeDataAsync(string jsonFilePath)
        {
            if (await _agences.CountDocumentsAsync(_ => true) == 0)
            {
                var json = await File.ReadAllTextAsync(jsonFilePath);
                var agences = JsonSerializer.Deserialize<List<Agence>>(json);
                if (agences != null)
                {
                    await _agences.InsertManyAsync(agences);
                }
            }
        }
    }
}