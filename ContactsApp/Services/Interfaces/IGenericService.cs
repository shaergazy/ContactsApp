namespace ContactsApp.Services.Interfaces
{
    public interface IGenericService <TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Retrieves all TEntitys from the database.
        /// </summary>
        /// <returns>An enumerable collection of TEntitys.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Adds a new TEntity to the database.
        /// </summary>
        /// <param name="TEntity">The TEntity to be added.</param>
       Task AddAsync(TEntity TEntity);

        /// <summary>
        /// Updates an existing TEntity in the database.
        /// </summary>
        /// <param name="TEntity">The TEntity with updated information.</param>
        void Update(TEntity TEntity);

        /// <summary>
        /// Deletes a TEntity from the database based on the provided ID.
        /// </summary>
        /// <param name="id">The ID of the TEntity to be deleted.</param>
        Task DeleteAsync(TKey id);
    }
}
