using System.Data;
using System.Linq.Expressions;
using Carpo.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Carpo.Data.EFCore.RepositoryBase
{
    public class RepositoryBaseQuery<TEntity> : RepositoryBaseQuery where TEntity : class
    {
        #region Constructor

        /// <summary>
        /// Contructor
        /// </summary>
        public RepositoryBaseQuery(DbContext contexto) : base(contexto) { }
        #endregion
    }

    /// <summary>
    /// Base repository class with methods for data querying.
    /// </summary>    
    public class RepositoryBaseQuery
    {
        #region Properties and Attributes
        private readonly DbContext _context;
        protected DbContext Context { get { return _context; } }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public RepositoryBaseQuery(DbContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods

        #region All

        /// <summary>
        /// Checks if all records of the entity meet the condition passed as a parameter.
        /// </summary>    
        public virtual async Task<bool> AllAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return await _context.Set<TEntity>().AllAsync(where);
        }

        /// <summary>
        /// Checks if all records of the entity meet the condition passed as a parameter.
        /// </summary>    
        public virtual bool All<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return _context.Set<TEntity>().All(where);
        }

        #endregion

        #region Any

        /// <summary>
        /// Checks if any record meets the specified condition.
        /// </summary>        
        public virtual bool Any<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return _context.Set<TEntity>().Any(where);
        }

        /// <summary>
        /// (Async) Checks if any record meets the specified condition.
        /// </summary>        
        public virtual async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return await _context.Set<TEntity>().AnyAsync(where);
        }

        #endregion

        #region GetSingleOrDefault

        /// <summary>
        /// Returns a single object of the entity type, otherwise returns NULL.
        /// </summary>                
        public TEntity GetSingleOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return _context.Set<TEntity>().SingleOrDefault(where);
        }

        /// <summary>
        /// Returns a single object of the entity type without related entity dependencies.
        /// </summary>                
        public TEntity GetSingleOrDefaultAsNoTracking<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return _context.Set<TEntity>().AsNoTracking().SingleOrDefault(where);
        }

        /// <summary>
        /// (Async) Returns a single object of the entity type without related entity dependencies.
        /// </summary>                
        public async Task<TEntity> GetSingleOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return await _context.Set<TEntity>().SingleOrDefaultAsync(where);
        }

        /// <summary>
        /// (Async and AsNoTracking) Returns a single object of the entity type without related entity dependencies.
        /// </summary>                
        public async Task<TEntity> GetSingleOrDefaultAsyncAsNoTracking<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return await _context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(where);
        }
        #endregion

        #region GetSingleById

        /// <summary>
        /// Locates an entity by its primary key (id).
        /// If the entity is already attached to the context, returns it without making a new request to the database.
        /// If not attached, searches the database. If found, attaches and returns the entity. Otherwise returns null.
        /// </summary>        
        public TEntity GetSingleById<TEntity>(object id) where TEntity : BaseDomain
        {
            return _context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// (Async) Locates an entity by its primary key (id).
        /// If the entity is already attached to the context, returns it without making a new request to the database.
        /// If not attached, searches the database. If found, attaches and returns the entity. Otherwise returns null.
        /// </summary>        
        public virtual async Task<TEntity> GetSingleByIdAsync<TEntity>(object id) where TEntity : BaseDomain
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        #endregion

        #region GetFirstOrDefault

        /// <summary>
        /// Returns the first element of a sequence that satisfies the given condition or returns null if no elements satisfy.
        /// </summary>       
        public TEntity GetFirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return _context.Set<TEntity>().FirstOrDefault(where);
        }

        /// <summary>
        /// (Async and AsNoTracking) Returns the first element of a sequence that satisfies the given condition or returns null if no elements satisfy.
        /// </summary>  
        public virtual async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(where);
        }

        #endregion

        #region GetAll

        /// <summary>
        /// Returns all records of a specific entity. Optionally, navigation properties can be passed to be loaded in the same query through inner join (eager loading).       
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The list of entity records.</returns>
        protected IQueryable<TEntity> GetAll<TEntity>() where TEntity : BaseDomain
        {
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// Returns all records of a specific entity identified in the method. 
        /// Optionally, navigation properties can be passed to be loaded in the same query through inner join (eager loading).       
        /// </summary>
        /// <typeparam name="TOtherEntity">The type of another entity.</typeparam>
        /// <returns>The list of records of the entity identified in the method.</returns>
        private IQueryable<TOtherEntity> GetAllEntity<TOtherEntity>() where TOtherEntity : class
        {
            return _context.Set<TOtherEntity>();
        }

        /// <summary>
        /// Returns all records of a specific entity disabling the tracking of the entity. This way, any changes made will not be identified. 
        /// Recommended for general selects for being more performatic. Optionally, navigation properties can be passed to be loaded in the same 
        /// query through inner join (eager loading).       
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>        
        /// <returns>The list of entity records.</returns>
        protected virtual IQueryable<TEntity> GetAllAsNoTracking<TEntity>() where TEntity : BaseDomain
        {
            return GetAll<TEntity>().AsNoTracking();
        }

        #endregion

        #region GetWhere

        /// <summary> 
        /// Returns the records according to the specified condition. If the condition is not specified, 
        /// returns NULL. 
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="where">The WHERE clause.</param>
        /// <returns>The list of entity records.</returns>
        public IQueryable<TEntity> GetWhere<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            if (where == null) { return null; }
            return _context.Set<TEntity>().Where(where);
        }

        /// <summary> 
        /// Returns all records of a specific entity according to the passed condition, disabling the tracking of the entity. 
        /// Recommended for general selects for being more performatic.      
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="where">The WHERE clause.</param> 
        /// <returns>The list of entity records.</returns>
        public virtual IQueryable<TEntity> GetWhereAsNoTracking<TEntity>(Expression<Func<TEntity, bool>> where) where TEntity : BaseDomain
        {
            // If no filter is passed, return all 
            if (where == null) { return null; }

            return _context.Set<TEntity>().AsNoTracking<TEntity>().Where(where);
        }

        #endregion

        #region GetUsingSqlQuery

        /// <summary>
        /// Executes an SQL command (query) and returns according to the type of the entity identified in the method.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>The list of entity records.</returns> 
        protected IQueryable<TEntity> GetUsingSqlQuery<TEntity>(string sqlCommand = "", params object[] sqlParams) where TEntity : BaseDomain
        {
            if (String.IsNullOrEmpty(sqlCommand)) { return null; }
            return GetUsingSqlQueryEntity<TEntity>(sqlCommand, sqlParams);
        }

        /// <summary>
        /// Executes an SQL command (query) and returns according to the type of another entity identified in the method.
        /// </summary>
        /// <typeparam name="TOtherEntity">The type of another entity.</typeparam>
        /// <returns>The list of records of the entity identified in the method.</returns> 
        protected IQueryable<TOtherEntity> GetUsingSqlQueryEntity<TOtherEntity>(string sqlCommand = "", params object[] sqlParams) where TOtherEntity : class
        {
            if (String.IsNullOrEmpty(sqlCommand)) { return null; }

            return _context.Set<TOtherEntity>().FromSqlRaw(sqlCommand, sqlParams);
        }

        #endregion

        #endregion
    }
}
