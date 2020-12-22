using System.Linq;
using System.Threading.Tasks;
using IdentityControl.API.Services.ToasterEvents;

namespace IdentityControl.API.Data
{
    public interface IIdentityRepository<T> where T : class
    {
        /// <summary>
        ///     Gets the query on the database entities.
        /// </summary>
        /// <returns>The query.</returns>
        IQueryable<T> Query();

        /// <summary>
        ///     Gets the entity by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        ///     Inserts the specified entity into the database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(T entity);

        /// <summary>
        ///     Inserts the specified entity into the database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        Task InsertAsync(T entity);

        /// <summary>
        ///     Deletes the entity coresponding to the specified id.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void Delete(object id);

        /// <summary>
        ///     Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(T entity);

        /// <summary>
        ///     Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Update(T entity);

        /// <summary>
        ///     Saves the modified entities to the database.
        /// </summary>
        void Save();

        /// <summary>
        ///     Saves the modified entities to the database.
        /// </summary>
        Task<int> SaveAsync();

        /// <summary>
        ///     Saves the modified entities to the database. And if the operation succeeded sends an event
        /// </summary>
        Task<int> SaveAsync(IToasterEvent toasterEvent, int expectedResult = 1);
    }
}