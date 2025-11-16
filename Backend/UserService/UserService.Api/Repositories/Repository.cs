namespace UserService.Api.Repositories;

public interface IRepository<TEntity, in TId>
    where TEntity : BaseEntity, IEntity<TId>
{
    Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken, 
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        bool asTracking = default);

    Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = default,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = default,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        bool asTracking = default);
    
    Task<PaginatedData<TEntity>> GetPaginatedAsync(
        int pageNumber, int pageSize,
        CancellationToken cancellationToken,
        Expression<Func<TEntity, bool>>? predicate = default,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = default,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
        bool asTracking = default);
    
    Task AddAsync(TEntity entity, CancellationToken cancellationToken);

    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken);

    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken);
}

public abstract class Repository<TEntity, TId> 
    : IRepository<TEntity, TId> 
    where TEntity : BaseEntity, IEntity<TId>
    {
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly DbContext _dbContext;

        protected Repository(UserServiceDbContext dbContext)
        {
            _dbSet = dbContext.Set<TEntity>();
            _dbContext = dbContext;
        }
        
        public async Task<TEntity?> GetByIdAsync(
            TId id, 
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
            bool asTracking = default)
        {
            return asTracking
                ? include is null
                    ? await _dbSet.FindAsync(new object[] {id}, cancellationToken)
                    : await include(_dbSet).AsTracking().SingleOrDefaultAsync(x => Equals(x.Id, id), cancellationToken)
                : include is null
                    ? await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => Equals(x.Id, id), cancellationToken)
                    : await include(_dbSet).AsNoTracking().SingleOrDefaultAsync(x => Equals(x.Id, id), cancellationToken);
        }
        
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>>? predicate = default,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = default,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default,
            bool asTracking = default)
        {
            var query = asTracking ? _dbSet.AsTracking() : _dbSet.AsNoTrackingWithIdentityResolution();

            query = include is null ? query : include(query);
            query = predicate is null ? query : query.Where(predicate);
            query = orderBy is null ? query : orderBy(query);

            return await query
                .ToListAsync(cancellationToken);
        }
        
        public async Task<PaginatedData<TEntity>> GetPaginatedAsync(
            int pageNumber, int pageSize,
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>>? predicate = default,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = default,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = default, 
            bool asTracking = default)
        {
            var query = _dbSet
                .AsNoTracking();
            var countQuery = _dbSet
                .AsNoTracking();
        
            query = include is null ? query : include(query);
            query = predicate is null ? query : query.Where(predicate);
            query = orderBy is null ? query : orderBy(query);
            
            countQuery = predicate is null ? countQuery  : countQuery.Where(predicate);
        
            var list = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            var totalCount = await countQuery
                .CountAsync(cancellationToken);
        
            var isHaveNextPage = totalCount > pageNumber * pageSize;
            var isHavePrevPage = pageNumber > 1;
            
            return new PaginatedData<TEntity>(
                Data: list,
                TotalCount: totalCount,
                IsHaveNextPage: isHaveNextPage,
                IsHavePrevPage: isHavePrevPage);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }


        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
            
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }