using GymSys.DAL.Data.DbContexts;
using GymSys.DAL.Data.Models;
using GymSys.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;

        private readonly Dictionary<string, object> _repositories = [];

        public UnitOfWork(GymDbContext dbContext, ISessionRepository sessionRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
        }

        public ISessionRepository SessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var typeName = typeof(TEntity).Name;
            
            if (_repositories.TryGetValue(typeName, out var repository))
            {
                return (IGenericRepository<TEntity>)repository;
            }
            else
            {
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repo;
                return repo;
            }

        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _dbContext.SaveChangesAsync(ct);
        }
    }
}
