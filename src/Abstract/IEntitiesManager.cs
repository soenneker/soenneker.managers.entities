using Soenneker.Entities.Entity;
using Soenneker.Exceptions.Suite;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Dtos.RequestDataOptions;
using Soenneker.Managers.Base.Abstract;

namespace Soenneker.Managers.Entities.Abstract;

/// <summary>
/// An abstract generic manager class provides CRUD operations for entities mapped to Cosmos DB documents
/// </summary>
public interface IEntitiesManager<TEntity> : IBaseManager where TEntity : Entity, new()
{
    /// <summary>
    /// Creates a new entity and stores it in the underlying data store.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The created entity, with updated values such as generated ID.</returns>
    ValueTask<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity by its identifier.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity corresponding to the given ID.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity is not found.</exception>
    [Pure]
    ValueTask<TEntity> Get(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of entities based on request options.
    /// </summary>
    /// <typeparam name="TResponse">The response DTO type (currently unused).</typeparam>
    /// <param name="options">The data options to control filtering, sorting, paging, etc.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of entities.</returns>
    [Pure]
    ValueTask<List<TEntity>> GetAll<TResponse>(RequestDataOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    /// <param name="entity">The entity with updated information.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated entity.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity to update is not found.</exception>
    ValueTask<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity from the data store by ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity to delete is not found.</exception>
    ValueTask Delete(string id, CancellationToken cancellationToken = default);
}