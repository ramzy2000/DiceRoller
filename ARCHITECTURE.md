# Architecture Overview - Dice Roller Application

## System Architecture

The Dice Roller application follows a **layered clean architecture** pattern, ensuring separation of concerns, testability, and maintainability.

## Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                        UI Layer                             │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Blazor Components (Components/)                      │  │
│  │  - DiceRoller.razor: Main UI                          │  │
│  │  - MainLayout.razor: App layout                       │  │
│  │  - NavMenu.razor: Navigation                          │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                    Business Logic Layer                     │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Services (Services/)                                 │  │
│  │  - IDiceRollerService: Dice rolling interface         │  │
│  │  - DiceRollerService: Rolling implementation          │  │
│  │  - IDiceGameService: Game logic interface             │  │
│  │  - DiceGameService: Game logic + persistence          │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                        Data Layer                           │
│  ┌───────────────────────────────────────────────────────┐  │
│  │  Data Access (Data/)                                  │  │
│  │  - DiceRollerDbContext: SQLite context               │  │
│  │                                                       │  │
│  │  Models (Models/)                                     │  │
│  │  - DiceRoll: Entity model                            │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            ↓ ↑
┌─────────────────────────────────────────────────────────────┐
│                    SQLite Database                          │
│                  (diceroller.db3)                           │
└─────────────────────────────────────────────────────────────┘
```

## Layer Details

### 1. UI Layer (Presentation)

**Location**: `Components/`

**Responsibilities**:
- User interface rendering
- User input handling
- Display logic
- Navigation

**Key Components**:
- **DiceRoller.razor**: Main page for dice rolling functionality
  - Dice selection (common dice + custom)
  - Roll button
  - Result display with animation
  - History list with recent rolls
  
**Technologies**:
- Blazor for component-based UI
- Razor syntax for templating
- CSS for styling
- Dependency injection for services

**Example Flow**:
```
User clicks "Roll d6" 
  → DiceRoller.razor calls GameService.RollAndSaveAsync(6)
  → Displays result with animation
  → Updates history list
```

### 2. Business Logic Layer

**Location**: `Services/`

**Responsibilities**:
- Core business rules
- Dice rolling logic
- Roll history management
- Input validation

**Key Services**:

#### DiceRollerService
```csharp
public interface IDiceRollerService
{
    int Roll(int sides);
    int MinSides { get; }
    int MaxSides { get; }
}
```
- Generates random numbers for dice rolls
- Validates dice sides (1-100)
- Uses System.Random for randomization

#### DiceGameService
```csharp
public interface IDiceGameService
{
    Task<DiceRoll> RollAndSaveAsync(int sides);
    Task<List<DiceRoll>> GetHistoryAsync();
    Task<List<DiceRoll>> GetRecentRollsAsync(int count);
    Task ClearHistoryAsync();
    Task<int> GetTotalRollsAsync();
}
```
- Orchestrates rolling and persistence
- Manages roll history
- Combines DiceRollerService with DiceRollerDbContext

### 3. Data Layer

**Location**: `Data/`, `Models/`

**Responsibilities**:
- Data persistence
- Database operations
- Entity models

**Key Components**:

#### DiceRollerDbContext
```csharp
public class DiceRollerDbContext
{
    Task<int> SaveRollAsync(DiceRoll roll);
    Task<List<DiceRoll>> GetAllRollsAsync();
    Task<List<DiceRoll>> GetRecentRollsAsync(int count);
    Task<int> ClearAllRollsAsync();
    Task<int> GetRollCountAsync();
}
```
- Manages SQLite database connection
- Provides async database operations
- Handles table creation and migrations

#### DiceRoll Entity
```csharp
public class DiceRoll
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int Sides { get; set; }
    public int Result { get; set; }
    public DateTime Timestamp { get; set; }
}
```

### 4. Database Layer

**Technology**: SQLite
**File**: `diceroller.db3` (in app data directory)

**Schema**:
```sql
CREATE TABLE DiceRoll (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Sides INTEGER NOT NULL,
    Result INTEGER NOT NULL,
    Timestamp DATETIME NOT NULL
);

CREATE INDEX idx_timestamp ON DiceRoll(Timestamp DESC);
```

## Dependency Injection

The application uses .NET's built-in dependency injection configured in `MauiProgram.cs`:

```csharp
// Register dice rolling service
builder.Services.AddSingleton<IDiceRollerService, DiceRollerService>();

// Register database context with file path
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "diceroller.db3");
builder.Services.AddSingleton(sp => new DiceRollerDbContext(dbPath));

// Register game service
builder.Services.AddSingleton<IDiceGameService, DiceGameService>();
```

**Lifetime Strategies**:
- **Singleton**: All services are registered as singletons since:
  - DiceRollerService has no state (Random is thread-safe enough for this use)
  - DiceRollerDbContext maintains connection pooling
  - DiceGameService orchestrates other singletons

## Data Flow

### Rolling a Die

```
1. User Input (UI Layer)
   └─> DiceRoller.razor: User selects d6 and clicks Roll
   
2. Service Call (Business Logic)
   └─> DiceGameService.RollAndSaveAsync(6)
       ├─> DiceRollerService.Roll(6) → generates random 1-6
       └─> Creates DiceRoll entity
   
3. Persistence (Data Layer)
   └─> DiceRollerDbContext.SaveRollAsync(roll)
       └─> SQLite INSERT operation
   
4. Return Result
   └─> Returns DiceRoll entity to UI
   
5. UI Update
   └─> DiceRoller.razor displays result with animation
   └─> Refreshes history list
```

### Loading History

```
1. Component Initialization (UI Layer)
   └─> DiceRoller.razor: OnInitializedAsync()
   
2. Service Call (Business Logic)
   └─> DiceGameService.GetRecentRollsAsync(10)
   
3. Data Retrieval (Data Layer)
   └─> DiceRollerDbContext.GetRecentRollsAsync(10)
       └─> SQLite SELECT with ORDER BY Timestamp DESC LIMIT 10
   
4. Return Data
   └─> Returns List<DiceRoll>
   
5. UI Rendering
   └─> DiceRoller.razor displays history list
```

## Design Patterns

### 1. Repository Pattern
- `DiceRollerDbContext` acts as a repository for `DiceRoll` entities
- Encapsulates all database operations
- Provides clean interface for data access

### 2. Service Layer Pattern
- Business logic separated from UI and data access
- Services provide cohesive operations
- Facilitates testing and reusability

### 3. Dependency Injection Pattern
- Loose coupling between components
- Easy testing with mock implementations
- Centralized service configuration

### 4. MVVM-inspired Pattern
- Blazor components act as ViewModels
- Separation of concerns between UI and logic
- Data binding through Blazor's @bind directive

## Testing Strategy

### Unit Tests

**DiceRollerServiceTests**:
- Tests roll range validation
- Tests edge cases (min/max sides)
- Tests randomness distribution
- Tests invalid input handling

**DiceRollerDbContextTests**:
- Tests CRUD operations
- Tests query ordering
- Tests data persistence
- Tests cleanup operations

### Integration Tests (Future)
- End-to-end roll workflow
- UI component testing
- Platform-specific features

### Manual Testing
- Cross-platform UI/UX
- Performance on real devices
- Database migration scenarios

## Performance Considerations

### Optimizations
1. **Database**: 
   - Indexed Timestamp column for fast sorting
   - Connection pooling via SQLite
   
2. **UI**: 
   - Limited history display (10 recent)
   - Async operations to prevent UI blocking
   
3. **Memory**: 
   - Singleton services reduce allocations
   - Database context reused

### Scalability
- Current design supports thousands of rolls
- For larger datasets, consider:
  - Pagination in history
  - Archive old rolls
  - Data pruning strategies

## Security Considerations

1. **Input Validation**:
   - Dice sides restricted to 1-100
   - UI-level and service-level validation

2. **Data Storage**:
   - SQLite database stored in app-private directory
   - No sensitive data stored

3. **Dependencies**:
   - Regular updates of NuGet packages
   - Vulnerability scanning with GitHub Advisory Database

## Future Architecture Enhancements

1. **Multi-dice Rolling**:
   - Add `DiceSet` model
   - Support expressions like "3d6+2"

2. **Cloud Sync**:
   - Add abstraction for cloud storage
   - Implement sync service

3. **Analytics**:
   - Add statistics calculation service
   - Roll distribution analysis

4. **Theming**:
   - Add theme service
   - Support light/dark modes

5. **Caching**:
   - Add in-memory cache for recent rolls
   - Reduce database queries

## Cross-Platform Considerations

The architecture is designed to work identically across all supported platforms:

- **Android**: SQLite in Android app data directory
- **iOS**: SQLite in iOS documents directory
- **macOS**: SQLite in Mac Catalyst documents
- **Windows**: SQLite in Windows app data

Platform-specific code is isolated in MAUI's `Platforms/` folder and doesn't affect core architecture.

## Maintenance Guidelines

1. **Adding New Features**:
   - Add interface to appropriate service
   - Implement in service class
   - Update UI components
   - Add tests

2. **Database Changes**:
   - Add migration logic in DiceRollerDbContext
   - Test on all platforms
   - Provide backward compatibility

3. **UI Updates**:
   - Modify Blazor components
   - Update CSS for styling
   - Test responsive behavior

4. **Service Changes**:
   - Update interface
   - Implement changes
   - Update dependent code
   - Run full test suite
