using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using Soenneker.Cosmos.Repository.Abstract;
using Soenneker.Documents.Document;
using Soenneker.Dtos.RequestDataOptions;
using Soenneker.Entities.Entity;
using Soenneker.Exceptions.Suite;
using Soenneker.Extensions.ValueTask;
using Soenneker.Managers.Base;
using Soenneker.Managers.Entities.Abstract;
using Soenneker.Redis.Util.Abstract;
using Soenneker.Utils.UserContext.Abstract;

namespace Soenneker.Managers.Entities;

///<inheritdoc cref="IEntitiesManager{TEntity}"/>
public abstract class EntitiesManager<TEntity, TDocument> : BaseManager, IEntitiesManager<TEntity> where TEntity : Entity, new() where TDocument : Document
{
    /// <summary>
    /// The repository made available from ctor
    /// </summary>
    protected ICosmosRepository<TDocument> Repo { get; }

    protected EntitiesManager(ICosmosRepository<TDocument> repo, IRedisUtil redisUtil, ILogger<EntitiesManager<TEntity, TDocument>> logger, IUserContext userContext)
        : base(redisUtil, logger, userContext)
    {
        Repo = repo;
    }

    public virtual async ValueTask<TEntity> Create(TEntity entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;

        var document = entity.Adapt<TDocument>();

        document.DocumentId = Guid.NewGuid().ToString();
        document.PartitionKey = document.DocumentId;

        string returnedId = await Repo.AddItem(document, cancellationToken: CancellationToken.None).NoSync();

        entity.Id = returnedId;

        return entity;
    }

    public virtual async ValueTask<TEntity> Get(string id, CancellationToken cancellationToken = default)
    {
        TDocument? document = await Repo.GetItem(id, cancellationToken).NoSync();

        if (document == null)
            throw new EntityNotFoundException(typeof(TEntity), id);

        return document.Adapt<TEntity>();
    }

    public virtual async ValueTask<List<TEntity>> GetAll<TResponse>(RequestDataOptions options, CancellationToken cancellationToken = default)
    {
        List<TDocument> docs = await Repo.GetAll(null, cancellationToken).NoSync();

        List<TEntity> result = new(docs.Count);

        for (var i = 0; i < docs.Count; i++)
        {
            TDocument doc = docs[i];
            result.Add(doc.Adapt<TEntity>());
        }

        return result;
    }

    public virtual async ValueTask<TEntity> Update(TEntity entity, CancellationToken cancellationToken = default)
    {
        TDocument? existingDocument = await Repo.GetItem(entity.Id, cancellationToken).NoSync();

        if (existingDocument == null)
            throw new EntityNotFoundException(typeof(TEntity), entity.Id);


        entity.ModifiedAt = DateTime.UtcNow;

        var toUpdateDocument = entity.Adapt<TDocument>();

        TDocument updatedDocument = await Repo.UpdateItem(entity.Id, toUpdateDocument, cancellationToken: CancellationToken.None).NoSync();

        return updatedDocument.Adapt<TEntity>();
    }

    public virtual async ValueTask Delete(string id, CancellationToken cancellationToken = default)
    {
        TDocument? document = await Repo.GetItem(id, cancellationToken).NoSync();

        if (document == null)
            throw new EntityNotFoundException(typeof(TEntity), id);

        await Repo.DeleteItem(id, cancellationToken: CancellationToken.None).NoSync();
    }
}