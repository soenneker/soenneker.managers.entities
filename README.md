[![](https://img.shields.io/nuget/v/soenneker.managers.entities.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.managers.entities/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.managers.entities/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.managers.entities/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.managers.entities.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.managers.entities/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.managers.entities/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.managers.entities/actions/workflows/codeql.yml)

# Soenneker.Managers.Entities

An abstract generic manager class provides CRUD operations for entities mapped to Cosmos DB documents.

## Install

```bash
dotnet add package Soenneker.Managers.Entities
```

## Quick start

```csharp
using Soenneker.Managers.Entities.Abstract;

IEntitiesManager<TEntity> entitiesManager = /* resolve from DI */;
var result = await entitiesManager.Create(/* supply entity */ default!, default);
```

Creates a new entity and stores it in the underlying data store.

## What you get

- `IEntitiesManager<TEntity>` — An abstract generic manager class provides CRUD operations for entities mapped to Cosmos DB documents.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IEntitiesManager<TEntity>.Create(entity, cancellationToken)` | Creates a new entity and stores it in the underlying data store. | The created entity, with updated values such as generated ID. |
| `IEntitiesManager<TEntity>.Get(id, cancellationToken)` | Retrieves a single entity by its identifier. | The entity corresponding to the given ID. |
| `IEntitiesManager<TEntity>.GetAll(options, cancellationToken)` | Retrieves a list of entities based on request options. | A list of entities. |
| `IEntitiesManager<TEntity>.Update(entity, cancellationToken)` | Updates an existing entity in the data store. | The updated entity. |
| `IEntitiesManager<TEntity>.Delete(id, cancellationToken)` | Deletes an entity from the data store by ID. | A task representing the asynchronous operation. |

## Important behavior

- `IEntitiesManager<TEntity>.Get(id, cancellationToken)`: Thrown if the entity is not found.
- `IEntitiesManager<TEntity>.Update(entity, cancellationToken)`: Thrown if the entity to update is not found.
- `IEntitiesManager<TEntity>.Delete(id, cancellationToken)`: Thrown if the entity to delete is not found.

## Practical notes

- Cancellation stops pending work; it does not undo work that has already completed.
