using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PicBook.Domain;
using PicBook.Domain.Exceptions;

namespace PicBook.Repository.EntityFramework
{
    public abstract class GenericCrudRepository<TEntity> where TEntity : class, IEntity
    {
        protected ApplicationDbContext Context;

        public async Task<IReadOnlyCollection<TEntity>> FindAll(Expression<Func<TEntity, bool>> filterExpression)
        {
            IQueryable<TEntity> entities = Context.Set<TEntity>();
            if (filterExpression != null)
            {
                entities = entities.Where(filterExpression);
            }

            return await entities.ToListAsync();
        }

        public virtual async Task<TEntity> FindById(Guid id)
        {
            return await Context.Set<TEntity>().FirstOrDefaultAsync(d => d.Id == id);
        }

        public virtual async Task<TEntity> GetById(Guid id)
        {
            TEntity entity = await FindById(id);
            if (entity == null)
            {
                throw new NotFoundException();
            }
            return entity;
        }

        public virtual async Task<TInheritedEntity> FindById<TInheritedEntity>(Guid id)
            where TInheritedEntity : class, TEntity
        {
            return await Context.Set<TInheritedEntity>().FirstOrDefaultAsync(d => d.Id == id);
        }

        public virtual async Task<TInheritedEntity> GetById<TInheritedEntity>(Guid id)
            where TInheritedEntity : class, TEntity
        {
            TInheritedEntity entity = await FindById<TInheritedEntity>(id);
            if (entity == null)
            {
                throw new NotFoundException();
            }
            return entity;
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }

            return await Context.Set<TEntity>().CountAsync(predicate);
        }

        public virtual async Task Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            
            Context.Set<TEntity>().Attach(entity);
            Context.Set<TEntity>().Add(entity);
            await Context.SaveChangesAsync();
        }

        public virtual async Task Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            
            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            await Context.SaveChangesAsync();
        }

        public virtual async Task Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Deleted;
            await Context.SaveChangesAsync();
        }
    }
}