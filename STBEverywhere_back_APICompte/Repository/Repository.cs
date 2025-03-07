using Microsoft.EntityFrameworkCore;
using STBEverywhere_Back_SharedModels.Data;
using STBEverywhere_back_APICompte.Repository.IRepository;
using STBEverywhere_Back_SharedModels;

namespace STBEverywhere_back_APICompte.Repository
{
    public class Repository<T>:IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbset;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbset=_db.Set<T>(); 
        }

        public async Task CreateAsync(T enyity)
        {
            await dbset.AddAsync(enyity);
            await SaveAsync();
        }


        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
