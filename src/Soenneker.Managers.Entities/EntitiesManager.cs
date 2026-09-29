using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Repository.Abstract;
using Soenneker.Constants.Data;
using Soenneker.Documents.Document;
using Soenneker.Dtos.RequestDataOptions;
using Soenneker.Dtos.Results.Paged;
using Soenneker.Entities.Entity;
using Soenneker.Exceptions.Suite;
using Soenneker.Extensions.ValueTask;
using Soenneker.Managers.Base;
using Soenneker.Managers.Entities.Abstract;
using Soenneker.Redis.Util.Abstract;
using Soenneker.Utils.UserContext.Abstract;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Cosmos.Repository.Dtos;

namespace Soenneker.Managers.Entities;

public abstract class EntitiesManager<TEntity, TDocument> : BaseManager, IEntitiesManager<TEntity>
    where TEntity : Entity, new() where TDocument : Document
{
    /// <summary>
    /// The repository made available from ctor
    /// </summary>
    protected ICosmosRepository<TDocument> Repo { get; }

    protected EntitiesManager(ICosmosRepository<TDocument> repo, IRedisUtil redisUtil,
        ILogger<EntitiesManager<TEntity, TDocument>> logger, IUserContext userContext) : base(redisUtil, logger,
        userContext)
    {
        Repo = repo;
    }

    /// <summary>
    /// Maps an entity to a new document for persistence.
    /// </summary>
    /// <param name="entity">The entity to map.</param>
    /// <returns>The mapped document.</returns>
    /// <remarks>Implement using concrete source and destination types to enable source-generated mapping without reflection.</remarks>
    protected abstract TDocument ToDocument(TEntity entity);

    /// <summary>
    /// Maps a stored document to a new entity.
    /// </summary>
    /// <param name="document">The document to map.</param>
    /// <returns>The mapped entity.</returns>
    /// <remarks>Implement using concrete source and destination types to enable source-generated mapping without reflection.</remarks>
    protected abstract TEntity ToEntity(TDocument document);

    public virtual async ValueTask<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTimeOffset.UtcNow;

        TDocument document = ToDocument(entity);

        document.DocumentId = Guid.NewGuid().ToString();
        document.PartitionKey = document.DocumentId;

        string returnedId = await Repo.AddItem(document, cancellationToken: cancellationToken).NoSync();

        entity.Id = returnedId;

        return entity;
    }

    public virtual async ValueTask<TEntity> Get(string id, CosmosReadOptions? cosmosReadOptions = null,
        CancellationToken cancellationToken = default)
    {
        TDocument? document = await Repo.GetItem(id, cosmosReadOptions, cancellationToken).NoSync();

        if (document == null)
            throw new EntityNotFoundException(typeof(TEntity), id);

        return ToEntity(document);
    }

    public virtual async ValueTask<List<TEntity>> GetAll(CosmosReadOptions? cosmosReadOptions = null,
        CancellationToken cancellationToken = default)
    {
        List<TDocument> docs = await Repo.GetAll(readOptions: cosmosReadOptions, cancellationToken: cancellationToken)
                                         .NoSync();

        List<TEntity> result = new(docs.Count);

        for (var i = 0; i < docs.Count; i++)
        {
            result.Add(ToEntity(docs[i]));
        }

        return result;
    }

    public virtual async ValueTask<PagedResult<TEntity>> GetAllPaged(RequestDataOptions options,
        CosmosReadOptions? cosmosReadOptions = null, CancellationToken cancellationToken = default)
    {
        int pageSize = options.PageSize > 0 ? options.PageSize : DataConstants.DefaultCosmosPageSize;

        (List<TDocument> docs, string? continuationToken) = await Repo.GetAllPaged(pageSize: pageSize,
            continuationToken: options.ContinuationToken, readOptions: cosmosReadOptions,
            cancellationToken: cancellationToken).NoSync();

        List<TEntity> result = new(docs.Count);

        for (var i = 0; i < docs.Count; i++)
        {
            TDocument doc = docs[i];
            result.Add(ToEntity(doc));
        }

        PagedResult<TEntity> pagedResult = new()
        {
            Items = result,
            PageSize = pageSize,
            ContinuationToken = continuationToken
        };

        return pagedResult;
    }

    public virtual async ValueTask<TEntity> Update(TEntity entity, CosmosReadOptions? cosmosReadOptions = null,
        CosmosWriteOptions? cosmosWriteOptions = null, CancellationToken cancellationToken = default)
    {
        TDocument? existingDocument = await Repo.GetItem(entity.Id, cosmosReadOptions, cancellationToken).NoSync();

        if (existingDocument == null)
            throw new EntityNotFoundException(typeof(TEntity), entity.Id);


        entity.ModifiedAt = DateTimeOffset.UtcNow;

        TDocument toUpdateDocument = ToDocument(entity);

        TDocument updatedDocument = await Repo.UpdateItem(entity.Id, toUpdateDocument, writeOptions: cosmosWriteOptions,
            cancellationToken: cancellationToken).NoSync();

        return ToEntity(updatedDocument);
    }

    public virtual async ValueTask Delete(string id, CosmosReadOptions? cosmosReadOptions = null,
        CosmosWriteOptions? cosmosWriteOptions = null, CancellationToken cancellationToken = default)
    {
        TDocument? document = await Repo.GetItem(id, cosmosReadOptions, cancellationToken).NoSync();

        if (document == null)
            throw new EntityNotFoundException(typeof(TEntity), id);

        await Repo.DeleteItem(id, writeOptions: cosmosWriteOptions, cancellationToken: cancellationToken).NoSync();
    }
}