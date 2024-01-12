using Carpo.Core.Domain;
using Carpo.Core.Domain.Extension;
using Microsoft.EntityFrameworkCore;

namespace Carpo.Data.EFCore.RepositoryBase
{
    public class RepositoryBaseCommand<TEntity> : RepositoryBaseQuery where TEntity : BaseDomain
    {
        #region Constructor
        /// <summary>
        /// Constructor for RepositoryBaseCommand.
        /// </summary>
        /// <param name="context">The database context.</param>            
        public RepositoryBaseCommand(DbContext context) : base(context)
        {
        }
        #endregion
    }
    /// <summary>
    /// Repository class for handling database commands, extends RepositoryBaseQuery.
    /// </summary>
    public class RepositoryBaseCommand : RepositoryBaseQuery
    {
        #region Constructor
        /// <summary>
        /// Constructor for RepositoryBaseCommand.
        /// </summary>
        /// <param name="context">The database context.</param>            
        public RepositoryBaseCommand(DbContext context) : base(context)
        {
        }
        #endregion

        #region Methods

        #region Save

        /// <summary>
        /// Inserts or updates the entity in the database.
        /// </summary>
        /// <param name="entity">The entity to be inserted or updated.</param>
        /// <param name="isAsync">Flag indicating whether the operation is asynchronous.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        /// <returns>The inserted or updated entity.</returns>
        private async Task<TEntity> SaveBase<TEntity>(TEntity entity, bool isAsync, bool saveChanges = true) where TEntity : BaseDomain
        {
            try
            {
                if (entity == null) return null;
                if (!entity.IsValidDomain)
                {
                    return entity;
                }

                object[] primaryKeyValues = entity.GetListPrimaryKey();

                TEntity entityToUpdate = Context.Set<TEntity>().Find(primaryKeyValues);

                if (entityToUpdate == null)
                {
                    Context.Set<TEntity>().Add(entity);
                }
                else
                {
                    Context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
                    Context.Entry(entityToUpdate).State = EntityState.Modified;
                }

                if (saveChanges)
                {
                    if (isAsync)
                    {
                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        Context.SaveChanges();
                    }
                }

                return entity;
            }
            catch (Exception exc)
            {
                // TODO: Handle the exception appropriately.
                throw exc;
            }
        }

        /// <summary>
        /// Inserts a new entity or updates changes if the entity was previously queried and attached to the context.
        /// </summary>
        /// <param name="entity">The entity to be saved or updated.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        /// <returns>The saved or updated entity. In the case of an insert, the entity returns with the newly assigned id.</returns>
        public TEntity Save<TEntity>(TEntity entity, bool saveChanges = true) where TEntity : BaseDomain
        {
            Task.Factory
                .StartNew(() => SaveBase(entity, false, saveChanges))
                .Unwrap<TEntity>()
                .GetAwaiter()
                .GetResult();

            return entity;
        }

        /// <summary>
        /// (Async) Inserts a new entity or updates changes if the entity was previously queried and attached to the context.
        /// </summary>
        /// <param name="entity">The entity to be saved or updated.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        /// <returns>The saved or updated entity. In the case of an insert, the entity returns with the newly assigned id.</returns>
        public async Task<TEntity> SaveAsync<TEntity>(TEntity entity, bool saveChanges = true) where TEntity : BaseDomain
        {
            return await SaveBase(entity, true, saveChanges);
        }
        #endregion

        #region Delete

        /// <summary>
        /// Deletes the entity from the database.
        /// </summary>
        private async Task DeleteBase<TEntity>(TEntity entity, bool saveChanges, bool isAsync) where TEntity : BaseDomain
        {
            try
            {
                if (entity == null) return;

                if (Context.Entry(entity).State == EntityState.Detached)
                {
                    Context.Set<TEntity>().Attach(entity);
                }

                Context.Set<TEntity>().Remove(entity);

                if (saveChanges)
                {
                    if (isAsync)
                    {
                        await Context.SaveChangesAsync();
                    }
                    else
                    {
                        Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException?.Message ?? ex.Message);
            }
        }

        /// <summary>
        /// Deletes one or more records based on the specified condition.
        /// </summary>
        /// <param name="where">The deletion condition.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        public void Delete<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> where, bool saveChanges = true) where TEntity : BaseDomain
        {
            var itemsToDelete = GetWhere(where);

            foreach (TEntity entity in itemsToDelete)
            {
                Task.Factory
                   .StartNew(async () => await DeleteBase(entity, saveChanges, false))
                   .GetAwaiter()
                   .GetResult();
            }

            if (saveChanges) Context.SaveChanges();
        }

        /// <summary>
        /// Removes an entity from the database that is already loaded in the context.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        public void Delete<TEntity>(TEntity entity, bool saveChanges = true) where TEntity : BaseDomain
        {
            Task.Factory
                   .StartNew(async () => await DeleteBase(entity, saveChanges, false))
                   .GetAwaiter()
                   .GetResult();
        }

        /// <summary>
        /// Removes an entity by its id. Internally, the method will retrieve the entity, attach it to the context, and then remove it.
        /// </summary>
        /// <param name="id">The id of the entity to be removed.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        public void DeleteById<TEntity>(object id, bool saveChanges = true) where TEntity : BaseDomain
        {
            Task.Factory
                   .StartNew(async () => await DeleteBase(GetSingleById<TEntity>(id), saveChanges, false))
                   .GetAwaiter()
                   .GetResult();
        }

        /// <summary>
        /// (Async) Deletes one or more records based on the specified condition.
        /// </summary>
        /// <param name="where">The deletion condition.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        public async Task DeleteAsync<TEntity>(System.Linq.Expressions.Expression<Func<TEntity, bool>> where, bool saveChanges = true) where TEntity : BaseDomain
        {
            var itemsToDelete = GetWhere(where);

            foreach (TEntity entity in itemsToDelete)
            {
                await DeleteBase(entity, saveChanges, false);
            }

            if (saveChanges) await SaveChangesAsync();
        }

        /// <summary>
        /// (Async) Removes an entity from the database that is already loaded in the context.
        /// </summary>
        /// <param name="entity">The entity to be removed.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        public async Task DeleteAsync<TEntity>(TEntity entity, bool saveChanges = true) where TEntity : BaseDomain
        {
            await DeleteBase(entity, saveChanges, false);
        }

        /// <summary>
        /// (Async) Removes an entity by its id. Internally, the method will retrieve the entity, attach it to the context, and then remove it.
        /// </summary>
        /// <param name="id">The id of the entity to be removed.</param>
        /// <param name="saveChanges">Flag indicating whether to save changes immediately.</param>
        public async Task DeleteByIdAsync<TEntity>(object id, bool saveChanges = true) where TEntity : BaseDomain
        {
            await DeleteBase(GetSingleById<TEntity>(id), saveChanges, false);
        }

        #endregion

        #region SaveChanges

        /// <summary>
        /// Saves the operations in the context to the database.
        /// </summary>
        public void SaveChanges()
        {
            base.Context.SaveChanges();
        }

        /// <summary>
        /// (Async) Saves the operations in the context to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await base.Context.SaveChangesAsync();
        }

        #endregion

        #endregion
    }
}
