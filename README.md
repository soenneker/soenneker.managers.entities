[![](https://img.shields.io/nuget/v/soenneker.managers.entities.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.managers.entities/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.managers.entities/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.managers.entities/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.managers.entities.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.managers.entities/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.managers.entities/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.managers.entities/actions/workflows/codeql.yml)

# Soenneker.Managers.Entities

An abstract CRUD manager that maps application entities to Cosmos DB documents.

## Install

```bash
dotnet add package Soenneker.Managers.Entities
```

## Usage

```csharp
public interface IOrdersManager : IEntitiesManager<Order>
{
}

public sealed class OrdersManager : EntitiesManager<Order, OrderDocument>, IOrdersManager
{
    public OrdersManager(
        ICosmosRepository<OrderDocument> repository,
        IRedisUtil redis,
        ILogger<EntitiesManager<Order, OrderDocument>> logger,
        IUserContext userContext)
        : base(repository, redis, logger, userContext)
    {
    }
}
```

The entity and document types must expose compatible properties for `AdaptViaReflection`. Register the derived manager—normally scoped—and ensure the matching `ICosmosRepository<TDocument>` is registered.

```csharp
Order created = await orders.Create(new Order { Name = "Sample" }, cancellationToken);
Order loaded = await orders.Get(created.Id, cancellationToken);
loaded.Name = "Updated";
await orders.Update(loaded, cancellationToken);
await orders.Delete(loaded.Id, cancellationToken);
```

## What you get

- `IEntitiesManager<TEntity>` — An abstract generic manager class provides CRUD operations for entities mapped to Cosmos DB documents.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IEntitiesManager<TEntity>.Create(entity, cancellationToken)` | Creates a new entity and stores it in the underlying data store. | The created entity, with updated values such as generated ID. |
| `IEntitiesManager<TEntity>.Get(id, cancellationToken)` | Retrieves a single entity by its identifier. | The entity corresponding to the given ID. |
| `IEntitiesManager<TEntity>.GetAll<TResponse>(options, cancellationToken)` | Retrieves documents and maps them to entities. | A `PagedResult<TEntity>`; the base implementation only uses `PageSize`. |
| `IEntitiesManager<TEntity>.Update(entity, cancellationToken)` | Updates an existing entity in the data store. | The updated entity. |
| `IEntitiesManager<TEntity>.Delete(id, cancellationToken)` | Deletes an entity from the data store by ID. | A task representing the asynchronous operation. |

## Important behavior

- `IEntitiesManager<TEntity>.Get(id, cancellationToken)`: Thrown if the entity is not found.
- `IEntitiesManager<TEntity>.Update(entity, cancellationToken)`: Thrown if the entity to update is not found.
- `IEntitiesManager<TEntity>.Delete(id, cancellationToken)`: Thrown if the entity to delete is not found.

## Practical notes

- `Create()` assigns a new GUID as both document id and partition key, sets `CreatedAt`, and replaces the entity's `Id` with the repository result.
- `Update()` sets `ModifiedAt`. `Get()`, `Update()`, and `Delete()` throw `EntityNotFoundException` when the id is absent.
- The base `GetAll<TResponse>()` does not apply continuation tokens, ordering, search, filters, counts, or `TResponse`; override it when those semantics are required.
- Caller cancellation is passed through to every Cosmos operation.
