using Soenneker.Cosmos.Repository.Dtos;
using Soenneker.Dtos.RequestDataOptions;
using Soenneker.Dtos.Results.Paged;
using Soenneker.Entities.Entity;
using Soenneker.Exceptions.Suite;
using Soenneker.Managers.Base.Abstract;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
    /// <param name="cosmosReadOptions">Overrides repository read defaults. Null inherits them; an explicit empty value restores SDK defaults.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The entity corresponding to the given ID.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity is not found.</exception>
    [Pure]
    ValueTask<TEntity> Get(string id, CosmosReadOptions? cosmosReadOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all entities from the repository.
    /// </summary>
    /// <param name="cosmosReadOptions">Overrides repository read defaults. Null inherits them; an explicit empty value restores SDK defaults.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>All documents mapped to entities.</returns>
    /// <remarks>Loads the full result set into memory. Use <see cref="GetAllPaged"/> to retrieve one page at a time.</remarks>
    [Pure]
    ValueTask<List<TEntity>> GetAll(CosmosReadOptions? cosmosReadOptions = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves one page of entities ordered by creation time descending.
    /// </summary>
    /// <param name="options">The requested page size and continuation token. Nonpositive page sizes use the repository's default page size.</param>
    /// <param name="cosmosReadOptions">Overrides repository read defaults. Null inherits them; an explicit empty value restores SDK defaults.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The mapped entities, effective page size, and continuation token for the next page, or null when paging is complete.</returns>
    /// <remarks>
    /// A page may contain fewer items than requested, including no items, while still having a continuation token.
    /// Continue using the returned token until it is null. Filtering, search, custom sorting, and total counts are not applied by the base implementation.
    /// </remarks>
    [Pure]
    ValueTask<PagedResult<TEntity>> GetAllPaged(RequestDataOptions options, CosmosReadOptions? cosmosReadOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the data store.
    /// </summary>
    /// <param name="entity">The entity with updated information.</param>
    /// <param name="cosmosReadOptions">Overrides repository read defaults. Null inherits them; an explicit empty value restores SDK defaults.</param>
    /// <param name="cosmosWriteOptions">Can require ETags for this call; cannot disable the repository ETag requirement. Unconditional writes are rejected when ETags are required.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The updated entity.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity to update is not found.</exception>
    ValueTask<TEntity> Update(TEntity entity, CosmosReadOptions? cosmosReadOptions = null, CosmosWriteOptions? cosmosWriteOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity from the data store by ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="cosmosReadOptions">Overrides repository read defaults. Null inherits them; an explicit empty value restores SDK defaults.</param>
    /// <param name="cosmosWriteOptions">Can require ETags for this call; cannot disable the repository ETag requirement. Unconditional writes are rejected when ETags are required.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="EntityNotFoundException">Thrown if the entity to delete is not found.</exception>
    ValueTask Delete(string id, CosmosReadOptions? cosmosReadOptions = null, CosmosWriteOptions? cosmosWriteOptions = null,
        CancellationToken cancellationToken = default);
}
