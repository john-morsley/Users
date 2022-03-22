﻿using Morsley.UK.People.Application.Models;

namespace Morsley.UK.People.Persistence.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<Guid>
    {
        protected readonly IMongoContext entitycontext;
        protected readonly IMongoCollection<TEntity> entitycollection;

        protected Repository(IMongoContext context, string tableName)
        {
            entitycontext = context;
            entitycollection = entitycontext.GetCollection<TEntity>(tableName);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                return await entitycollection.Find<TEntity>(entity => entity.Id == id).AnyAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async virtual Task<TEntity> GetByIdAsync(Guid id)
        {
            try
            {
                return await entitycollection.Find<TEntity>(entity => entity.Id == id).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async virtual Task<IPagedList<TEntity>> GetPageAsync(IGetOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            var entities = await GetAllAsync(options);

            var pagedList = PagedList<TEntity>.Create(entities, options.PageNumber, options.PageSize);

            return pagedList;
        }

        public async virtual Task AddAsync(TEntity entity)
        {
            // ToDo --> Set Created
            try
            {
                await entitycollection.InsertOneAsync(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async virtual Task UpdateAsync(TEntity update)
        {
            // ToDo --> Set Updated
            try
            {
                await entitycollection.ReplaceOneAsync(entity => entity.Id == update.Id, update);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async virtual Task DeleteAsync(Guid id)
        {
            try
            {
                await entitycollection.DeleteOneAsync(entity => entity.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected async virtual Task<IQueryable<TEntity>> GetAllAsync(IGetOptions options)
        {
            var entities = await Task.Run(() => AsQueryable());

            entities = Sort(entities, options);
            entities = Filter(entities, options);
            entities = Search(entities, options);

            return entities;
        }

        protected IQueryable<TEntity> AsQueryable()
        {
            return entitycollection.AsQueryable();
        }

        protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IGetOptions options)
        {
            return entities;
        }

        protected virtual IQueryable<TEntity> Search(IQueryable<TEntity> entities, IGetOptions options)
        {
            return entities;
        }

        protected virtual IQueryable<TEntity> Sort(IQueryable<TEntity> entities, IGetOptions options)
        {
            if (!options.Orderings.Any()) return entities;

            return entities.OrderBy(ToOrderByString(options.Orderings));
        }

        private string ToOrderByString(IEnumerable<IOrdering> orderings)
        {
            var orderBys = new List<string>();

            foreach (var ordering in orderings)
            {
                var orderBy = ordering.Key;
                switch (ordering.Order)
                {
                    case SortOrder.Ascending:
                        orderBy += " asc";
                        break;
                    case SortOrder.Descending:
                        orderBy += " desc";
                        break;
                }
                orderBys.Add(orderBy);
            }

            return string.Join(",", orderBys);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}