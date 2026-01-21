# Generic Guide: Building a CQRS Module with Aggregate Root Pattern

> **Purpose**: This is a reusable template for AI assistants to build any domain module using CQRS pattern and Domain-Driven Design principles. Replace placeholders with your specific domain concepts.

---

## Placeholders Reference

Throughout this guide, replace these placeholders with your domain-specific names:

| Placeholder | Description | Example |
|-------------|-------------|---------|
| `{ModuleName}` | The module/bounded context name | `Tasks`, `Orders`, `Inventory` |
| `{AggregateRoot}` | The main entity (aggregate root) | `TaskItem`, `Order`, `Product` |
| `{ChildEntity1}` | First child entity | `TaskMail`, `OrderLine`, `StockMovement` |
| `{ChildEntity2}` | Second child entity (if any) | `TaskStatusHistory`, `OrderPayment` |
| `{GrandchildEntity}` | Child of child entity (if any) | `TaskMailAttachment`, `OrderLineDiscount` |
| `{Enum1}`, `{Enum2}` | Domain enumerations | `TaskStatus`, `OrderStatus`, `Priority` |

---

## Table of Contents
1. [Phase 1: Domain Analysis](#phase-1-domain-analysis)
2. [Phase 2: Architecture Pattern](#phase-2-architecture-pattern)
3. [Phase 3: Design the Aggregate](#phase-3-design-the-aggregate)
4. [Phase 4: Project Structure](#phase-4-project-structure)
5. [Phase 5: Implementation Order](#phase-5-implementation-order)
6. [Phase 6: Code Templates](#phase-6-code-templates)
7. [Phase 7: Wiring Everything Together](#phase-7-wiring-everything-together)

---

## Phase 1: Domain Analysis

### Step 1.1: Identify Core Concepts

Before writing code, answer these questions:

**Question A: What are the nouns (entities)?**
```
List all the things your domain manages:
- {AggregateRoot}: The main concept
- {ChildEntity1}: Related concept owned by aggregate
- {ChildEntity2}: Another related concept
- {GrandchildEntity}: Concept owned by child entity
```

**Question B: What are the verbs (operations)?**
```
List all actions users can perform:
- Create {AggregateRoot}
- Update {AggregateRoot}
- Delete {AggregateRoot}
- Add {ChildEntity1} to {AggregateRoot}
- Change {Property} (e.g., status, priority)
- etc.
```

### Step 1.2: Define Relationships

Draw the relationship diagram:

```
{AggregateRoot} (1) ──────< {ChildEntity1} (many)
                                │
                                └──────< {GrandchildEntity} (many)

{AggregateRoot} (1) ──────< {ChildEntity2} (many)
```

**Template questions:**
- Does {AggregateRoot} own one or many {ChildEntity1}?
- Can {ChildEntity1} exist without {AggregateRoot}? (Should be NO)
- What is the lifecycle relationship?

### Step 1.3: Identify Business Rules

List all invariants and rules:

| Rule Description | Where to Enforce |
|------------------|------------------|
| {AggregateRoot} must have {required field} | {AggregateRoot}.Validate() |
| Cannot {action} when {condition} | {AggregateRoot}.{Method}() |
| {ChildEntity1} must be unique by {field} | {AggregateRoot}.Add{ChildEntity1}() |
| State transitions must follow {rules} | {AggregateRoot}.Change{State}() |
| Changes must be audited | {AggregateRoot}.{Method}() creates {HistoryEntity} |

---

## Phase 2: Architecture Pattern

### CQRS Overview

```
WRITE PATH (Commands):
Controller → Command → Handler → Aggregate → Repository → Database
                         ↓
                   (validates business rules)

READ PATH (Queries):
Controller → Query → Handler → Direct SQL/Dapper → DTO
                                   ↓
                          (optimized for display)
```

### Key Principles

1. **Commands** = Write operations that change state
2. **Queries** = Read operations that return data
3. **Aggregate Root** = Entry point for all modifications
4. **Repository** = Loads/Saves complete aggregates
5. **Handlers** = One handler per command/query (Single Responsibility)

### When to Use CQRS

✅ **Use when:**
- Complex business rules exist
- Read patterns differ from write patterns
- Audit trails are required
- Domain logic is non-trivial

❌ **Avoid when:**
- Simple CRUD with no business logic
- Prototype or throwaway code

---

## Phase 3: Design the Aggregate

### Step 3.1: Define the Aggregate Boundary

```
┌─────────────────────────────────────────┐
│           AGGREGATE BOUNDARY            │
│                                         │
│   ┌─────────────────┐                   │
│   │ {AggregateRoot} │ ← Aggregate Root  │
│   └────────┬────────┘                   │
│            │                            │
│     ┌──────┴──────┐                     │
│     ▼             ▼                     │
│ ┌────────────┐ ┌────────────┐           │
│ │{ChildEntity1}│ │{ChildEntity2}│       │
│ └──────┬─────┘ └────────────┘           │
│        ▼                                │
│ ┌──────────────────┐                    │
│ │{GrandchildEntity}│                    │
│ └──────────────────┘                    │
└─────────────────────────────────────────┘
```

**Rules:**
- Everything inside the boundary is loaded/saved together
- External entities reference aggregate by ID only
- All modifications go through the root

### Step 3.2: Define State Transitions (if applicable)

```
┌─────────┐     ┌────────────┐     ┌───────────┐
│ {State1}│ ──► │  {State2}  │ ──► │  {State3} │
└─────────┘     └────────────┘     └───────────┘
                     │
                     ▼
               ┌───────────┐
               │  {State4} │
               └───────────┘
```

Define valid transitions:
- {State1} → {State2}: Allowed
- {State1} → {State3}: Not Allowed (must go through {State2})
- {State3} → {State1}: Not Allowed (terminal state)

---

## Phase 4: Project Structure

### Standard Folder Layout

```
MyArch.Apps.{ModuleName}/
├── Contracts/                    ← Interfaces and DTOs
│   ├── I{AggregateRoot}Repository.cs
│   └── DTOs/
│       ├── PagedResponse.cs
│       ├── Requests/
│       │   ├── Create{AggregateRoot}Request.cs
│       │   ├── Update{AggregateRoot}Request.cs
│       │   ├── Change{Property}Request.cs
│       │   └── Add{ChildEntity1}Request.cs
│       └── Responses/
│           ├── {AggregateRoot}Dto.cs
│           ├── {AggregateRoot}DetailDto.cs
│           ├── {ChildEntity1}Dto.cs
│           └── {ChildEntity2}Dto.cs
├── Entities/                     ← Domain models
│   ├── IAggregateRoot.cs
│   ├── {ModuleName}Enums.cs
│   ├── {AggregateRoot}.cs
│   ├── {ChildEntity1}.cs
│   ├── {ChildEntity2}.cs
│   └── {GrandchildEntity}.cs
├── Exceptions/                   ← Domain exceptions
│   └── DomainExceptions.cs
├── Extensions/                   ← DI setup
│   ├── ServiceCollectionExtensions.cs
│   └── {ModuleName}Options.cs
└── Implements/                   ← Concrete implementations
    ├── Behaviors/
    │   ├── LoggingBehavior.cs
    │   └── ValidationBehavior.cs
    ├── Commands/
    │   ├── Create{AggregateRoot}Command.cs
    │   ├── Update{AggregateRoot}Command.cs
    │   ├── Delete{AggregateRoot}Command.cs
    │   ├── Change{Property}Command.cs
    │   └── Add{ChildEntity1}Command.cs
    ├── Queries/
    │   ├── Get{AggregateRoot}Query.cs
    │   ├── Get{AggregateRoot}DetailQuery.cs
    │   ├── Get{AggregateRoot}ListQuery.cs
    │   └── Get{ChildEntity1}sQuery.cs
    ├── Controllers/
    │   └── {ModuleName}Controller.cs
    ├── Repositories/
    │   └── {AggregateRoot}Repository.cs
    └── Validators/
        ├── Create{AggregateRoot}RequestValidator.cs
        ├── Update{AggregateRoot}RequestValidator.cs
        └── Add{ChildEntity1}RequestValidator.cs
```

---

## Phase 5: Implementation Order

### Build in this exact sequence:

```
Step 1:  Exceptions      → Define domain error types
Step 2:  Enums           → Define shared constants/states
Step 3:  Entities        → Build aggregate root + child entities
Step 4:  DTOs            → Define request/response shapes
Step 5:  Repository      → Define persistence interface + implementation
Step 6:  Commands        → Build write operations
Step 7:  Queries         → Build read operations
Step 8:  Validators      → Add FluentValidation rules
Step 9:  Behaviors       → Add MediatR pipeline behaviors
Step 10: Controller      → Wire up HTTP endpoints
Step 11: DI Extensions   → Register all services
Step 12: SQL Scripts     → Create database tables
```

---

## Phase 6: Code Templates

### 6.1 Exceptions Template

```csharp
// Exceptions/DomainExceptions.cs
namespace MyArch.Apps.{ModuleName}.Exceptions;

public class {ModuleName}DomainException : Exception
{
    public {ModuleName}DomainException(string message) : base(message) { }
}

public class {AggregateRoot}NotFoundException : {ModuleName}DomainException
{
    public {AggregateRoot}NotFoundException(Guid id) 
        : base($"{AggregateRoot} with ID {id} was not found.") { }
}

public class Invalid{Property}TransitionException : {ModuleName}DomainException
{
    public Invalid{Property}TransitionException(string from, string to)
        : base($"Cannot transition from {from} to {to}.") { }
}

public class DuplicateEntityException : {ModuleName}DomainException
{
    public DuplicateEntityException(string entityName, string identifier)
        : base($"{entityName} with identifier '{identifier}' already exists.") { }
}
```

### 6.2 Enums Template

```csharp
// Entities/{ModuleName}Enums.cs
namespace MyArch.Apps.{ModuleName}.Entities;

public enum {AggregateRoot}Status
{
    Draft = 0,
    Active = 1,
    Completed = 2,
    Cancelled = 3
}

public enum {AggregateRoot}Priority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}
```

### 6.3 Aggregate Root Template

```csharp
// Entities/{AggregateRoot}.cs
namespace MyArch.Apps.{ModuleName}.Entities;

public class {AggregateRoot} : IAggregateRoot
{
    // === Identity ===
    public Guid Id { get; private set; }
    
    // === Core Properties ===
    public string {RequiredField1} { get; private set; } = string.Empty;
    public string? {OptionalField1} { get; private set; }
    public {AggregateRoot}Status Status { get; private set; }
    public {AggregateRoot}Priority Priority { get; private set; }
    
    // === Audit Fields ===
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime? UpdatedAt { get; private set; }
    public string? UpdatedBy { get; private set; }
    
    // === Child Collections (Navigation) ===
    private readonly List<{ChildEntity1}> _{childEntity1}s = new();
    public IReadOnlyCollection<{ChildEntity1}> {ChildEntity1}s => _{childEntity1}s.AsReadOnly();
    
    private readonly List<{ChildEntity2}> _{childEntity2}s = new();
    public IReadOnlyCollection<{ChildEntity2}> {ChildEntity2}s => _{childEntity2}s.AsReadOnly();
    
    // === Factory Method (Creation) ===
    public static {AggregateRoot} Create(
        string {requiredField1},
        string? {optionalField1},
        {AggregateRoot}Priority priority,
        string createdBy)
    {
        var entity = new {AggregateRoot}
        {
            Id = Guid.NewGuid(),
            {RequiredField1} = {requiredField1},
            {OptionalField1} = {optionalField1},
            Status = {AggregateRoot}Status.Draft,
            Priority = priority,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };
        
        entity.Validate();
        return entity;
    }
    
    // === Behavior Methods ===
    public void Update(string {requiredField1}, string? {optionalField1}, string updatedBy)
    {
        {RequiredField1} = {requiredField1};
        {OptionalField1} = {optionalField1};
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
        
        Validate();
    }
    
    public void ChangeStatus({AggregateRoot}Status newStatus, string changedBy, string? reason = null)
    {
        ValidateStatusTransition(Status, newStatus);
        
        var history = new {ChildEntity2}(
            Guid.NewGuid(),
            Id,
            Status,
            newStatus,
            changedBy,
            reason);
        
        _{childEntity2}s.Add(history);
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = changedBy;
    }
    
    public void ChangePriority({AggregateRoot}Priority newPriority, string changedBy)
    {
        if (Status == {AggregateRoot}Status.Completed || Status == {AggregateRoot}Status.Cancelled)
            throw new {ModuleName}DomainException("Cannot change priority of a closed item.");
        
        Priority = newPriority;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = changedBy;
    }
    
    public void Add{ChildEntity1}({ChildEntity1} child)
    {
        // Check for duplicates by unique identifier
        if (_{childEntity1}s.Any(x => x.UniqueIdentifier == child.UniqueIdentifier))
            throw new DuplicateEntityException(nameof({ChildEntity1}), child.UniqueIdentifier);
        
        _{childEntity1}s.Add(child);
        UpdatedAt = DateTime.UtcNow;
    }
    
    // === Validation ===
    private void Validate()
    {
        if (string.IsNullOrWhiteSpace({RequiredField1}))
            throw new {ModuleName}DomainException("{RequiredField1} is required.");
    }
    
    private void ValidateStatusTransition({AggregateRoot}Status from, {AggregateRoot}Status to)
    {
        var allowed = (from, to) switch
        {
            ({AggregateRoot}Status.Draft, {AggregateRoot}Status.Active) => true,
            ({AggregateRoot}Status.Active, {AggregateRoot}Status.Completed) => true,
            ({AggregateRoot}Status.Active, {AggregateRoot}Status.Cancelled) => true,
            ({AggregateRoot}Status.Draft, {AggregateRoot}Status.Cancelled) => true,
            _ => false
        };
        
        if (!allowed)
            throw new Invalid{Property}TransitionException(from.ToString(), to.ToString());
    }
    
    // === For Repository (Reconstitution) ===
    public void Load{ChildEntity1}s(IEnumerable<{ChildEntity1}> items)
    {
        _{childEntity1}s.Clear();
        _{childEntity1}s.AddRange(items);
    }
    
    public void Load{ChildEntity2}s(IEnumerable<{ChildEntity2}> items)
    {
        _{childEntity2}s.Clear();
        _{childEntity2}s.AddRange(items);
    }
    
    // Private constructor for EF/Dapper
    private {AggregateRoot}() { }
}
```

### 6.4 Child Entity Template

```csharp
// Entities/{ChildEntity1}.cs
namespace MyArch.Apps.{ModuleName}.Entities;

public class {ChildEntity1}
{
    public Guid Id { get; private set; }
    public Guid {AggregateRoot}Id { get; private set; }
    
    // === Core Properties ===
    public string UniqueIdentifier { get; private set; } = string.Empty;
    public string {Field1} { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    
    // === Grandchild Collection (if any) ===
    private readonly List<{GrandchildEntity}> _{grandchildEntity}s = new();
    public IReadOnlyCollection<{GrandchildEntity}> {GrandchildEntity}s => _{grandchildEntity}s.AsReadOnly();
    
    // === Factory Method ===
    public static {ChildEntity1} Create(
        Guid {aggregateRoot}Id,
        string uniqueIdentifier,
        string {field1})
    {
        return new {ChildEntity1}
        {
            Id = Guid.NewGuid(),
            {AggregateRoot}Id = {aggregateRoot}Id,
            UniqueIdentifier = uniqueIdentifier,
            {Field1} = {field1},
            CreatedAt = DateTime.UtcNow
        };
    }
    
    public void Add{GrandchildEntity}({GrandchildEntity} item)
    {
        _{grandchildEntity}s.Add(item);
    }
    
    public void Load{GrandchildEntity}s(IEnumerable<{GrandchildEntity}> items)
    {
        _{grandchildEntity}s.Clear();
        _{grandchildEntity}s.AddRange(items);
    }
    
    private {ChildEntity1}() { }
}
```

### 6.5 Repository Interface Template

```csharp
// Contracts/I{AggregateRoot}Repository.cs
namespace MyArch.Apps.{ModuleName}.Contracts;

public interface I{AggregateRoot}Repository
{
    Task<{AggregateRoot}?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<{AggregateRoot}?> GetByIdWithChildrenAsync(Guid id, CancellationToken ct = default);
    Task SaveAsync({AggregateRoot} aggregate, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
```

### 6.6 Command Template

```csharp
// Implements/Commands/Create{AggregateRoot}Command.cs
namespace MyArch.Apps.{ModuleName}.Implements.Commands;

public record Create{AggregateRoot}Command(
    Create{AggregateRoot}Request Request,
    string UserId
) : IRequest<{AggregateRoot}Dto>;

public class Create{AggregateRoot}CommandHandler 
    : IRequestHandler<Create{AggregateRoot}Command, {AggregateRoot}Dto>
{
    private readonly I{AggregateRoot}Repository _repository;
    
    public Create{AggregateRoot}CommandHandler(I{AggregateRoot}Repository repository)
    {
        _repository = repository;
    }
    
    public async Task<{AggregateRoot}Dto> Handle(
        Create{AggregateRoot}Command command, 
        CancellationToken ct)
    {
        var aggregate = {AggregateRoot}.Create(
            command.Request.{RequiredField1},
            command.Request.{OptionalField1},
            command.Request.Priority,
            command.UserId);
        
        // Add child entities if provided
        foreach (var childDto in command.Request.{ChildEntity1}s ?? [])
        {
            var child = {ChildEntity1}.Create(
                aggregate.Id,
                childDto.UniqueIdentifier,
                childDto.{Field1});
            
            aggregate.Add{ChildEntity1}(child);
        }
        
        await _repository.SaveAsync(aggregate, ct);
        
        return MapToDto(aggregate);
    }
    
    private static {AggregateRoot}Dto MapToDto({AggregateRoot} entity) => new()
    {
        Id = entity.Id,
        {RequiredField1} = entity.{RequiredField1},
        Status = entity.Status.ToString(),
        CreatedAt = entity.CreatedAt
    };
}
```

### 6.7 Query Template

```csharp
// Implements/Queries/Get{AggregateRoot}ListQuery.cs
namespace MyArch.Apps.{ModuleName}.Implements.Queries;

public record Get{AggregateRoot}ListQuery(
    int Page = 1,
    int PageSize = 20,
    string? Status = null,
    string? SearchTerm = null
) : IRequest<PagedResponse<{AggregateRoot}Dto>>;

public class Get{AggregateRoot}ListQueryHandler 
    : IRequestHandler<Get{AggregateRoot}ListQuery, PagedResponse<{AggregateRoot}Dto>>
{
    private readonly IDbConnection _db;
    
    public Get{AggregateRoot}ListQueryHandler(IDbConnection db)
    {
        _db = db;
    }
    
    public async Task<PagedResponse<{AggregateRoot}Dto>> Handle(
        Get{AggregateRoot}ListQuery query, 
        CancellationToken ct)
    {
        var sql = @"
            SELECT Id, {RequiredField1}, Status, Priority, CreatedAt, CreatedBy
            FROM {TableName}
            WHERE (@Status IS NULL OR Status = @Status)
              AND (@SearchTerm IS NULL OR {RequiredField1} LIKE '%' + @SearchTerm + '%')
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
            
            SELECT COUNT(*)
            FROM {TableName}
            WHERE (@Status IS NULL OR Status = @Status)
              AND (@SearchTerm IS NULL OR {RequiredField1} LIKE '%' + @SearchTerm + '%');
        ";
        
        var offset = (query.Page - 1) * query.PageSize;
        
        using var multi = await _db.QueryMultipleAsync(sql, new
        {
            query.Status,
            query.SearchTerm,
            Offset = offset,
            query.PageSize
        });
        
        var items = (await multi.ReadAsync<{AggregateRoot}Dto>()).ToList();
        var totalCount = await multi.ReadSingleAsync<int>();
        
        return new PagedResponse<{AggregateRoot}Dto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }
}
```

### 6.8 Validator Template

```csharp
// Implements/Validators/Create{AggregateRoot}RequestValidator.cs
namespace MyArch.Apps.{ModuleName}.Implements.Validators;

public class Create{AggregateRoot}RequestValidator 
    : AbstractValidator<Create{AggregateRoot}Request>
{
    public Create{AggregateRoot}RequestValidator()
    {
        RuleFor(x => x.{RequiredField1})
            .NotEmpty().WithMessage("{RequiredField1} is required.")
            .MaximumLength(500).WithMessage("{RequiredField1} cannot exceed 500 characters.");
        
        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value.");
        
        RuleForEach(x => x.{ChildEntity1}s)
            .SetValidator(new {ChildEntity1}DtoValidator());
    }
}

public class {ChildEntity1}DtoValidator : AbstractValidator<{ChildEntity1}InputDto>
{
    public {ChildEntity1}DtoValidator()
    {
        RuleFor(x => x.UniqueIdentifier)
            .NotEmpty().WithMessage("Unique identifier is required.");
    }
}
```

### 6.9 Controller Template

```csharp
// Implements/Controllers/{ModuleName}Controller.cs
namespace MyArch.Apps.{ModuleName}.Implements.Controllers;

[ApiController]
[Route("api/v2/{moduleName}")]
public class {ModuleName}Controller : ControllerBase
{
    private readonly IMediator _mediator;
    
    public {ModuleName}Controller(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResponse<{AggregateRoot}Dto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var query = new Get{AggregateRoot}ListQuery(page, pageSize, status, search);
        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<{AggregateRoot}DetailDto>> GetById(
        Guid id, 
        CancellationToken ct)
    {
        var query = new Get{AggregateRoot}DetailQuery(id);
        var result = await _mediator.Send(query, ct);
        return result is null ? NotFound() : Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<{AggregateRoot}Dto>> Create(
        [FromBody] Create{AggregateRoot}Request request,
        CancellationToken ct)
    {
        var userId = User.Identity?.Name ?? "system";
        var command = new Create{AggregateRoot}Command(request, userId);
        var result = await _mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<{AggregateRoot}Dto>> Update(
        Guid id,
        [FromBody] Update{AggregateRoot}Request request,
        CancellationToken ct)
    {
        var userId = User.Identity?.Name ?? "system";
        var command = new Update{AggregateRoot}Command(id, request, userId);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new Delete{AggregateRoot}Command(id);
        await _mediator.Send(command, ct);
        return NoContent();
    }
    
    [HttpPost("{id:guid}/status")]
    public async Task<ActionResult<{AggregateRoot}Dto>> ChangeStatus(
        Guid id,
        [FromBody] Change{Property}Request request,
        CancellationToken ct)
    {
        var userId = User.Identity?.Name ?? "system";
        var command = new Change{Property}Command(id, request, userId);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
    
    [HttpPost("{id:guid}/{childEntity1}s")]
    public async Task<ActionResult<{AggregateRoot}Dto>> Add{ChildEntity1}(
        Guid id,
        [FromBody] Add{ChildEntity1}Request request,
        CancellationToken ct)
    {
        var command = new Add{ChildEntity1}Command(id, request);
        var result = await _mediator.Send(command, ct);
        return Ok(result);
    }
}
```

### 6.10 Repository Implementation Template

```csharp
// Implements/Repositories/{AggregateRoot}Repository.cs
namespace MyArch.Apps.{ModuleName}.Implements.Repositories;

public class {AggregateRoot}Repository : I{AggregateRoot}Repository
{
    private readonly IDbConnection _db;
    private readonly IUnitOfWork _uow;
    
    public {AggregateRoot}Repository(IDbConnection db, IUnitOfWork uow)
    {
        _db = db;
        _uow = uow;
    }
    
    public async Task<{AggregateRoot}?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
            SELECT * FROM {AggregateRoot}s WHERE Id = @Id;
        ";
        return await _db.QuerySingleOrDefaultAsync<{AggregateRoot}>(sql, new { Id = id });
    }
    
    public async Task<{AggregateRoot}?> GetByIdWithChildrenAsync(Guid id, CancellationToken ct)
    {
        const string sql = @"
            SELECT * FROM {AggregateRoot}s WHERE Id = @Id;
            SELECT * FROM {ChildEntity1}s WHERE {AggregateRoot}Id = @Id;
            SELECT * FROM {GrandchildEntity}s WHERE {ChildEntity1}Id IN 
                (SELECT Id FROM {ChildEntity1}s WHERE {AggregateRoot}Id = @Id);
            SELECT * FROM {ChildEntity2}s WHERE {AggregateRoot}Id = @Id ORDER BY CreatedAt;
        ";
        
        using var multi = await _db.QueryMultipleAsync(sql, new { Id = id });
        
        var aggregate = await multi.ReadSingleOrDefaultAsync<{AggregateRoot}>();
        if (aggregate is null) return null;
        
        var children1 = (await multi.ReadAsync<{ChildEntity1}>()).ToList();
        var grandchildren = (await multi.ReadAsync<{GrandchildEntity}>()).ToLookup(x => x.{ChildEntity1}Id);
        var children2 = (await multi.ReadAsync<{ChildEntity2}>()).ToList();
        
        foreach (var child in children1)
        {
            child.Load{GrandchildEntity}s(grandchildren[child.Id]);
        }
        
        aggregate.Load{ChildEntity1}s(children1);
        aggregate.Load{ChildEntity2}s(children2);
        
        return aggregate;
    }
    
    public async Task SaveAsync({AggregateRoot} aggregate, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        try
        {
            // Upsert aggregate root
            const string upsertSql = @"
                MERGE INTO {AggregateRoot}s AS target
                USING (SELECT @Id AS Id) AS source
                ON target.Id = source.Id
                WHEN MATCHED THEN
                    UPDATE SET {RequiredField1} = @{RequiredField1}, 
                               Status = @Status,
                               UpdatedAt = @UpdatedAt,
                               UpdatedBy = @UpdatedBy
                WHEN NOT MATCHED THEN
                    INSERT (Id, {RequiredField1}, Status, Priority, CreatedAt, CreatedBy)
                    VALUES (@Id, @{RequiredField1}, @Status, @Priority, @CreatedAt, @CreatedBy);
            ";
            await _db.ExecuteAsync(upsertSql, aggregate);
            
            // Delete existing children and re-insert
            await _db.ExecuteAsync(
                "DELETE FROM {ChildEntity1}s WHERE {AggregateRoot}Id = @Id", 
                new { aggregate.Id });
            
            foreach (var child in aggregate.{ChildEntity1}s)
            {
                await _db.ExecuteAsync(@"
                    INSERT INTO {ChildEntity1}s (Id, {AggregateRoot}Id, UniqueIdentifier, {Field1}, CreatedAt)
                    VALUES (@Id, @{AggregateRoot}Id, @UniqueIdentifier, @{Field1}, @CreatedAt)", 
                    child);
                
                foreach (var grandchild in child.{GrandchildEntity}s)
                {
                    await _db.ExecuteAsync(@"
                        INSERT INTO {GrandchildEntity}s (Id, {ChildEntity1}Id, ...)
                        VALUES (@Id, @{ChildEntity1}Id, ...)", 
                        grandchild);
                }
            }
            
            // Insert new history records
            foreach (var history in aggregate.{ChildEntity2}s.Where(h => h.Id == Guid.Empty || /* is new */))
            {
                await _db.ExecuteAsync(@"
                    INSERT INTO {ChildEntity2}s (Id, {AggregateRoot}Id, FromStatus, ToStatus, ChangedBy, Reason, CreatedAt)
                    VALUES (@Id, @{AggregateRoot}Id, @FromStatus, @ToStatus, @ChangedBy, @Reason, @CreatedAt)", 
                    history);
            }
            
            await _uow.CommitTransactionAsync(ct);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
    
    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await _uow.BeginTransactionAsync(ct);
        
        try
        {
            await _db.ExecuteAsync("DELETE FROM {GrandchildEntity}s WHERE {ChildEntity1}Id IN (SELECT Id FROM {ChildEntity1}s WHERE {AggregateRoot}Id = @Id)", new { Id = id });
            await _db.ExecuteAsync("DELETE FROM {ChildEntity1}s WHERE {AggregateRoot}Id = @Id", new { Id = id });
            await _db.ExecuteAsync("DELETE FROM {ChildEntity2}s WHERE {AggregateRoot}Id = @Id", new { Id = id });
            await _db.ExecuteAsync("DELETE FROM {AggregateRoot}s WHERE Id = @Id", new { Id = id });
            
            await _uow.CommitTransactionAsync(ct);
        }
        catch
        {
            await _uow.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
```

### 6.11 DI Extensions Template

```csharp
// Extensions/ServiceCollectionExtensions.cs
namespace MyArch.Apps.{ModuleName}.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection Add{ModuleName}Module(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register options
        services.Configure<{ModuleName}Options>(
            configuration.GetSection("{ModuleName}"));
        
        // Register repository
        services.AddScoped<I{AggregateRoot}Repository, {AggregateRoot}Repository>();
        
        // Register MediatR handlers from this assembly
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        });
        
        // Register validators
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        
        // Register behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        
        return services;
    }
}
```

---

## Phase 7: Wiring Everything Together

### 7.1 Flow Diagram: Create Operation

```
HTTP POST /api/v2/{moduleName}
        │
        ▼
┌───────────────────────┐
│  {ModuleName}Controller│
│  .Create()            │
└───────────┬───────────┘
            │ new Create{AggregateRoot}Command(request)
            ▼
┌───────────────────────┐
│      MediatR.Send()   │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│  ValidationBehavior   │ ◄── Validates request DTO
│  (Pipeline)           │
└───────────┬───────────┘
            │
            ▼
┌───────────────────────┐
│   LoggingBehavior     │ ◄── Logs request details
│   (Pipeline)          │
└───────────┬───────────┘
            │
            ▼
┌─────────────────────────────┐
│ Create{AggregateRoot}       │
│ CommandHandler.Handle()     │
│                             │
│  1. {AggregateRoot}.Create()│ ◄── Business rules validated
│  2. Add child entities      │
│  3. Repository.SaveAsync()  │ ◄── Transaction + persistence
│  4. Return DTO              │
└───────────┬─────────────────┘
            │
            ▼
    201 Created + {AggregateRoot}Dto
```

### 7.2 Flow Diagram: Read Operation

```
HTTP GET /api/v2/{moduleName}?page=1&status=Active
        │
        ▼
┌───────────────────────┐
│  {ModuleName}Controller│
│  .GetAll()            │
└───────────┬───────────┘
            │ new Get{AggregateRoot}ListQuery(...)
            ▼
┌───────────────────────┐
│      MediatR.Send()   │
└───────────┬───────────┘
            │
            ▼
┌─────────────────────────────┐
│ Get{AggregateRoot}List      │
│ QueryHandler.Handle()       │
│                             │
│  1. Build SQL with filters  │
│  2. Execute via Dapper      │ ◄── Direct SQL, no aggregate
│  3. Return PagedResponse    │
└───────────┬─────────────────┘
            │
            ▼
    200 OK + PagedResponse<{AggregateRoot}Dto>
```

### 7.3 SQL Schema Template

```sql
-- Tables for {ModuleName} module

CREATE TABLE {AggregateRoot}s (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    {RequiredField1} NVARCHAR(500) NOT NULL,
    {OptionalField1} NVARCHAR(MAX) NULL,
    Status INT NOT NULL DEFAULT 0,
    Priority INT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy NVARCHAR(256) NOT NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(256) NULL
);

CREATE TABLE {ChildEntity1}s (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    {AggregateRoot}Id UNIQUEIDENTIFIER NOT NULL,
    UniqueIdentifier NVARCHAR(500) NOT NULL,
    {Field1} NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_{ChildEntity1}_{AggregateRoot} 
        FOREIGN KEY ({AggregateRoot}Id) REFERENCES {AggregateRoot}s(Id)
);

CREATE TABLE {GrandchildEntity}s (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    {ChildEntity1}Id UNIQUEIDENTIFIER NOT NULL,
    {Field} NVARCHAR(MAX) NOT NULL,
    CONSTRAINT FK_{GrandchildEntity}_{ChildEntity1} 
        FOREIGN KEY ({ChildEntity1}Id) REFERENCES {ChildEntity1}s(Id)
);

CREATE TABLE {ChildEntity2}s (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    {AggregateRoot}Id UNIQUEIDENTIFIER NOT NULL,
    FromStatus INT NOT NULL,
    ToStatus INT NOT NULL,
    ChangedBy NVARCHAR(256) NOT NULL,
    Reason NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_{ChildEntity2}_{AggregateRoot} 
        FOREIGN KEY ({AggregateRoot}Id) REFERENCES {AggregateRoot}s(Id)
);

-- Indexes
CREATE INDEX IX_{AggregateRoot}s_Status ON {AggregateRoot}s(Status);
CREATE INDEX IX_{AggregateRoot}s_CreatedAt ON {AggregateRoot}s(CreatedAt DESC);
CREATE INDEX IX_{ChildEntity1}s_{AggregateRoot}Id ON {ChildEntity1}s({AggregateRoot}Id);
CREATE INDEX IX_{ChildEntity2}s_{AggregateRoot}Id ON {ChildEntity2}s({AggregateRoot}Id);
```

---

## Golden Rules Summary

| # | Rule | Rationale |
|---|------|-----------|
| 1 | Commands change state through aggregates | Business rules are enforced |
| 2 | Queries read state via direct SQL | Performance optimization |
| 3 | Aggregate root is the only entry point | Consistency boundary |
| 4 | Repository loads/saves complete aggregates | Atomic operations |
| 5 | One handler per command/query | Single Responsibility |
| 6 | Validators check format, entities check rules | Separation of concerns |
| 7 | Controller is thin (just translates HTTP) | Keep framework code minimal |
| 8 | All business logic lives in the aggregate | Domain model is the source of truth |

---

## Checklist for AI Implementation

When building a new module, verify:

- [ ] All entities identified and relationships mapped
- [ ] Aggregate boundary clearly defined
- [ ] Business rules listed and assigned to methods
- [ ] State transitions documented (if applicable)
- [ ] Exception types defined
- [ ] Enums created
- [ ] Aggregate root with factory method and behavior methods
- [ ] Child entities with factory methods
- [ ] Repository interface defined
- [ ] Repository implementation with transaction handling
- [ ] Command for each write operation
- [ ] Query for each read operation
- [ ] Validator for each request DTO
- [ ] Controller with proper HTTP verbs and routes
- [ ] DI registration complete
- [ ] SQL schema created with indexes

---

## Example: Applying to "Orders" Domain

Quick example of placeholder substitution:

| Placeholder | Orders Domain Value |
|-------------|---------------------|
| `{ModuleName}` | Orders |
| `{AggregateRoot}` | Order |
| `{ChildEntity1}` | OrderLine |
| `{ChildEntity2}` | OrderStatusHistory |
| `{GrandchildEntity}` | OrderLineDiscount |
| `{Enum1}` | OrderStatus |
| `{Enum2}` | PaymentStatus |
| `{RequiredField1}` | CustomerName |
| `{TableName}` | Orders |

This would generate:
- `Order.cs` (aggregate root)
- `OrderLine.cs` (child entity)
- `OrderStatusHistory.cs` (audit trail)
- `CreateOrderCommand.cs`
- `GetOrderListQuery.cs`
- etc.
