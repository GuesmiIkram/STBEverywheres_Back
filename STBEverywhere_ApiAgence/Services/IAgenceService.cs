using MongoDB.Driver;
using STBEverywhere_ApiAgence.Models;

namespace STBEverywhere_ApiAgence.Services
{
    public interface IAgenceService
    {
        IMongoCollection<Agence> Collection { get; }
    }

}
