# Tic-Tac-Toe WPF

A modern, feature-rich desktop implementation of the classic Tic-Tac-Toe game built with WPF, showcasing clean architecture principles and modern C# development practices.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=csharp)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?style=flat&logo=windows)
![Entity Framework](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=flat)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?style=flat&logo=microsoftsqlserver)
![Tests](https://img.shields.io/badge/Tests-xUnit-512BD4?style=flat)

## Overview

This project demonstrates proficiency in **enterprise-level WPF development**, combining modern UI/UX design with robust backend architecture. Built as a portfolio piece, it showcases best practices in software engineering, including clean code principles, SOLID design patterns, and comprehensive error handling.

**Project Status:** On Hold (Core features complete, future enhancements planned)

##  Key Features

### Gameplay
- **Multiple Game Modes**
  - Player vs Player (local multiplayer)
  - Player vs AI with three difficulty levels (Easy, Medium, Hard)
  - Real-time game state management
  
- **Intelligent AI Opponent**
  - Easy: Random moves
  - Medium: Strategic blocking with 50% smart play probability
  - Hard: Minimax algorithm implementation for optimal play

### Data Management
- **Comprehensive Statistics System**
  - Player profiles with automatic registration
  - Win/Loss/Draw tracking
  - Win rate calculation
  - Game history with timestamps and duration
  - Recent games view (last 10 games)

### User Experience
- **Modern, Polished UI**
  - Custom gradient-based design system
  - Smooth animations and hover effects
  - Intuitive navigation flow
  - Responsive layout

- **Internationalization (i18n)**
  - Multi-language support (English & Ukrainian)
  - Runtime language switching
  - Persistent language preferences
  - Fully localized UI strings

- **Settings Management**
  - Language preferences
  - Persistent configuration storage

##  Technical Highlights

This project demonstrates advanced development practices that are essential in professional software development:

### Architecture & Design Patterns
- **MVVM (Model-View-ViewModel)** - Clean separation of concerns with data binding
- **Repository Pattern** - Abstract data access layer for maintainability
- **Unit of Work Pattern** - Transaction management and consistency
- **Result Pattern** - Type-safe error handling without exceptions
- **Dependency Injection** - Loose coupling and testability via Microsoft.Extensions.DependencyInjection
- **Factory Pattern** - EF Core design-time factory for migrations

### Code Quality
- **Nullable Reference Types** - Enhanced null safety with C# 12
- **Async/Await** - Non-blocking operations throughout the application
- **SOLID Principles** - Single Responsibility, Interface Segregation, Dependency Inversion
- **Clean Code** - Meaningful naming, small focused methods, clear intent

### Error Handling
- **Result Pattern Implementation** - Explicit success/failure handling
- **Graceful Degradation** - User-friendly error messages
- **Comprehensive Validation** - Input validation at service layer
- **Exception Safety** - Proper exception handling with logging

### Testing
- **Unit Tests** - 64 test cases covering core business logic (in progress)
- **Mocking** - Moq framework for dependency isolation
- **Test Organization** - Separate test project with clear structure
- **Test Categories** - Pattern tests, service tests, integration tests

### Database
- **Entity Framework Core 10** - Modern ORM with fluent API configuration
- **Code-First Migrations** - Version-controlled database schema
- **Relationship Management** - Proper foreign key relationships
- **Indexing Strategy** - Optimized queries with strategic indexes

##  Architecture

### MVVM Pattern Implementation

```
┌─────────────────────────────────────────────────────────┐
│                         View (XAML)                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│  │ MainWindow   │  │  Statistics  │  │   Settings   │   │
│  └──────────────┘  └──────────────┘  └──────────────┘   │
└────────────────────────┬────────────────────────────────┘
                         │ Data Binding
                         ▼
┌─────────────────────────────────────────────────────────┐
│                      ViewModel Layer                    │
│  ┌──────────────────────────────────────────────────┐   │
│  │          GameViewModel (INPC)                    │   │
│  │  • ObservableCollection<string> Board            │   │
│  │  • ICommand CellClickCommand                     │   │
│  │  • ICommand RestartCommand                       │   │
│  └──────────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                     Service Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐   │
│  │PlayerService │  │GameResult    │  │GameEngine    │   │
│  │              │  │Service       │  │Service       │   │
│  └──────────────┘  └──────────────┘  └──────────────┘   │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  Repository Layer (UoW)                 │
│  ┌──────────────┐  ┌──────────────┐                     │
│  │Player        │  │GameResult    │                     │
│  │Repository    │  │Repository    │                     │
│  └──────────────┘  └──────────────┘                     │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              Entity Framework Core (ORM)                │
└────────────────────────┬────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│              SQL Server LocalDB                         │
│  ┌──────────────┐  ┌──────────────┐                     │
│  │Players Table │  │GameResults   │                     │
│  │              │  │Table         │                     │
│  └──────────────┘  └──────────────┘                     │
└─────────────────────────────────────────────────────────┘
```

### Database Schema

```sql
Players
├── Id (int, PK, Identity)
├── Name (nvarchar(100), Required, Indexed)
└── GameResults (Navigation Property)

GameResults
├── Id (int, PK, Identity)
├── PlayerX (int, FK → Players.Id, Required, Indexed)
├── PlayerO (int, FK → Players.Id, Required, Indexed)
├── Winner (nvarchar(50), Required) -- Player ID or "Draw"
├── Duration (time, Required)
├── PlayedAt (datetime2, Required, Indexed)
└── Player (Navigation Property)

Indexes:
- Players.Name (for quick player lookup)
- GameResults.PlayedAt (for chronological queries)
- GameResults.PlayerX (for player statistics)
- GameResults.PlayerO (for player statistics)
```

##  Technologies

### Frontend
- **WPF (Windows Presentation Foundation)** - Rich desktop UI framework
- **XAML** - Declarative markup for UI definition
- **Data Binding** - Two-way binding with `INotifyPropertyChanged`
- **Resource Dictionaries** - Centralized styling and localization
- **Custom Controls** - Styled buttons, textboxes, and radio buttons

### Backend
- **.NET 10.0** - Latest LTS framework with modern C# features
- **C# 12.0** - Latest language features including:
  - Nullable reference types
  - Required members
  - Collection expressions
  - Primary constructors (in tests)
- **Entity Framework Core 10.0** - Modern ORM with:
  - Fluent API configuration
  - Migration system
  - LINQ query support
  - Change tracking
- **SQL Server LocalDB** - Lightweight, development-focused database

### Architecture & Patterns
- **Microsoft.Extensions.DependencyInjection** - Built-in DI container
- **Microsoft.Extensions.Hosting** - Host builder for service configuration
- **Microsoft.Extensions.Configuration** - Configuration management
- **Microsoft.Extensions.Logging** - Structured logging support

### Testing
- **xUnit** - Modern testing framework
- **Moq 4.20** - Mocking library for unit tests
- **Coverlet** - Code coverage tool

### Development Tools
- **Visual Studio 2022** - Primary IDE
- **EF Core Tools** - Migration management
- **Git** - Version control

##  Getting Started

### Prerequisites

- **Windows 10/11** (64-bit)
- **.NET 10.0 SDK** - [Download here](https://dotnet.microsoft.com/download)
- **SQL Server Express LocalDB** - Included with Visual Studio or downloadable separately
- **Visual Studio 2022** (recommended) with:
  - .NET desktop development workload
  - SQL Server Express LocalDB component

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/Granuch/tic-tac-toe-wpf.git
   cd tic-tac-toe-wpf
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project Tic-Tac-Toe
   ```
   
   This will:
   - Create the LocalDB database
   - Apply all migrations
   - Set up tables and indexes

4. **Build the solution**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project Tic-Tac-Toe
   ```

### Using Visual Studio

1. Open `Tic-Tac-Toe.slnx` in Visual Studio 2022
2. Press `F5` to build and run
3. The database will be created automatically on first run

### Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

## Project Structure

```
Tic-Tac-Toe/
├── 📂 Constants/
│   └── GameConstants.cs              # Game configuration constants
├── 📂 DbContext/
│   ├── GameContext.cs                # EF Core DbContext
│   ├── DbContextFactory.cs           # Design-time factory
│   └── DatabaseConfig.cs             # Connection string configuration
├── 📂 Models/
│   ├── Player.cs                     # Player entity with validation
│   └── GameResult.cs                 # Game result entity
├── 📂 ViewModels/
│   ├── GameViewModel.cs              # Main game logic & state
│   ├── ObjectObserver.cs             # Base class for INPC
│   └── RelayCommand.cs               # ICommand implementation
├── 📂 Views/
│   ├── MainWindow.xaml               # Game board UI
│   ├── PlayerSelectionWindow.xaml    # Player setup UI
│   ├── StatisticsWindow.xaml         # Statistics display
│   └── SettingsWindow.xaml           # Settings UI
├── 📂 Services/
│   ├── Interfaces/
│   │   ├── IPlayerService.cs         # Player service contract
│   │   ├── IGameResultService.cs     # Game result service contract
│   │   ├── IGameEngine.cs            # Game engine contract
│   │   ├── IBotPlayerService.cs      # Bot player contract
│   │   ├── ILocalizationService.cs   # Localization contract
│   │   └── IAppLogger.cs             # Logging contract
│   ├── PlayerService.cs              # Player business logic
│   ├── GameResultService.cs          # Game result operations
│   ├── GameEngineService.cs          # Core game logic
│   ├── BotPlayerService.cs           # AI implementation
│   ├── LocalizationService.cs        # i18n support
│   └── AppLogger.cs                  # Logging implementation
├── 📂 Patterns/
│   ├── RepositoryPattern/
│   │   ├── IRepository.cs            # Generic repository interface
│   │   ├── Repository.cs             # Base repository implementation
│   │   ├── IPlayerRepository.cs      # Player-specific repository
│   │   ├── PlayerRepository.cs       # Player repository impl
│   │   ├── IGameResultRepository.cs  # GameResult-specific repository
│   │   ├── GameResultRepository.cs   # GameResult repository impl
│   │   ├── IUnitOfWork.cs            # Unit of Work interface
│   │   └── UnitOfWork.cs             # Transaction coordinator
│   └── ResultPattern/
│       └── ResultPattern.cs          # Result<T> implementation
├── 📂 Resources/
│   ├── Lang.en.xaml                  # English translations
│   └── Lang.uk.xaml                  # Ukrainian translations
├── 📂 Migrations/                    # EF Core migrations
├── App.xaml                          # Application styles & resources
├── App.xaml.cs                       # Application entry point & DI setup
└── appsettings.json                  # Configuration file

Tic-Tac-Toe.Tests/
├── 📂 Patterns/
│   └── ResultPatternTests.cs         # Result pattern unit tests
├── 📂 Services/
│   ├── PlayerServiceTests.cs         # Player service tests
│   └── GameResultServiceTests.cs     # Game result service tests
└── 📂 Integration/
    └── ResultPatternIntegrationTests.cs  # Integration tests
```

## Design Patterns

### 1. MVVM (Model-View-ViewModel)

**Purpose:** Separation of UI from business logic

```csharp
// ViewModel with INotifyPropertyChanged
public class GameViewModel : ObjectObserver
{
    private string _statusText;
    public string StatusText
    {
        get => _statusText;
        set { _statusText = value; OnPropertyChanged(); }
    }
    
    public ICommand CellClickCommand { get; }
}
```

**Benefits:**
- Testable business logic
- Data binding support
- Clear separation of concerns

### 2. Repository Pattern

**Purpose:** Abstract data access logic

```csharp
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
}

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<Player> GetOrCreateAsync(string name, CancellationToken ct = default);
}
```

**Benefits:**
- Testable data access
- Swappable data sources
- Single Responsibility Principle

### 3. Unit of Work Pattern

**Purpose:** Transaction management and coordination

```csharp
public interface IUnitOfWork : IDisposable
{
    IPlayerRepository Players { get; }
    IGameResultRepository GameResults { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
}
```

**Benefits:**
- Atomic operations
- Centralized transaction control
- Reduced database round-trips

### 4. Result Pattern

**Purpose:** Type-safe error handling

```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public string Error { get; }
}

// Usage
var result = await _playerService.GetOrCreatePlayerAsync(name);
if (result.IsFailure)
{
    ShowError(result.Error);
    return;
}
var player = result.Value;
```

**Benefits:**
- Explicit error handling
- No exception overhead for expected failures
- Railway-oriented programming support
- Better IDE support (no silent failures)

### 5. Dependency Injection

**Purpose:** Loose coupling and testability

```csharp
// Registration in App.xaml.cs
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped<IPlayerService, PlayerService>();
services.AddTransient<GameViewModel>();

// Usage in constructor
public class GameViewModel
{
    private readonly IGameEngine _engine;
    private readonly IPlayerService _playerService;
    
    public GameViewModel(
        IGameEngine gameEngine,
        IPlayerService playerService)
    {
        _engine = gameEngine;
        _playerService = playerService;
    }
}
```

**Benefits:**
- Easy testing with mocks
- Flexible component replacement
- Clear dependency graph

## Testing

### Test Coverage

The project includes comprehensive unit and integration tests covering:

-  **Result Pattern** (20+ tests)
  - Success/Failure scenarios
  - Generic type handling
  - Null value support
  - Immutability verification

-  **Player Service** (15+ tests)
  - Player creation and retrieval
  - Name validation
  - Error handling
  - Edge cases

-  **Game Result Service** (20+ tests)
  - Game result saving
  - Statistics calculation
  - History retrieval
  - Win rate computation

-  **Integration Tests** (10+ tests)
  - Result chain propagation
  - Realistic scenarios
  - Error message verification
  - Type safety validation

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test category
dotnet test --filter "FullyQualifiedName~PlayerServiceTests"

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Examples

```csharp
[Fact]
public async Task GetOrCreatePlayerAsync_WithValidName_ShouldReturnSuccess()
{
    // Arrange
    var playerName = "TestPlayer";
    var expectedPlayer = new Player { Id = 1, Name = playerName };
    _mockPlayerRepo.Setup(r => r.GetOrCreateAsync(playerName, It.IsAny<CancellationToken>()))
        .ReturnsAsync(expectedPlayer);

    // Act
    var result = await _service.GetOrCreatePlayerAsync(playerName);

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
    Assert.Equal(playerName, result.Value.Name);
}
```

##  Future Enhancements

Potential improvements and features for future iterations:

### Gameplay
- [ ] **Game Modes**
  - Timed matches
  - Tournament mode
  - Custom board sizes (4x4, 5x5)

- [ ] **Animations**
  - Win line highlighting
  - Piece placement effects
  - Screen transitions

### Features
- [ ] **Online Multiplayer**
  - SignalR real-time communication
  - Matchmaking system
  - Friend lists

- [ ] **Social Features**
  - Global leaderboard
  - Achievements system
  - Player profiles with avatars

- [ ] **Customization**
  - Theme system (Dark/Light modes)
  - Custom color schemes
  - Sound effects and music
  - Board skins

### Technical
- [ ] **Export & Import**
  - Game replay system
  - Statistics export (PDF/Excel)
  - Save game states

##  Contributing

Contributions, issues, and feature requests are welcome!

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Contribution Guidelines

- Follow existing code style and conventions
- Write unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR

##  License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

##  Contact

**Granuch**
- GitHub: [@Granuch](https://github.com/Granuch)
- Project Link: https://github.com/Granuch/Tic-Tac-Toe

⭐ **If you find this project helpful, please consider giving it a star!**
