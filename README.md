# Tic-Tac-Toe WPF

A modern desktop implementation of the classic Tic-Tac-Toe game built with WPF (Windows Presentation Foundation) and Entity Framework Core.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=csharp)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?style=flat&logo=windows)
![Entity Framework](https://img.shields.io/badge/EF%20Core-10.0-512BD4?style=flat)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?style=flat&logo=microsoftsqlserver)

## Table of Contents

- [Features](#-features)
- [Technologies](#-technologies)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [Database](#-database)
- [Project Structure](#-project-structure)
- [Future Enhancements](#-future-enhancements)
- [License](#-license)

## Features

### Core Gameplay
-  **Player vs Player Mode** - Play against a friend locally
-  **Bot Mode** - Challenge AI opponents with three difficulty levels
-  **Quick Restart** - Start a new game instantly
-  **Game Timer** - Track how long each game takes

### Data Persistence
-  **Player Profiles** - Automatic player registration and tracking
-  **Statistics Tracking** - View detailed stats for each player
-  **Game History** - Review past games with timestamps and durations
-  **Win Rate Calculation** - See your performance metrics

### User Interface
-  **Clean Design** - Modern, intuitive interface
-  **Responsive Layout** - Optimized for desktop experience
-  **Localized** - Ukrainian language interface
-  **Real-time Updates** - Instant board updates using MVVM pattern
  
##  Technologies

### Frontend
- **WPF** - Windows Presentation Foundation for rich UI
- **XAML** - Declarative UI markup
- **Data Binding** - Two-way binding with MVVM pattern

### Backend
- **.NET 10.0** - Latest .NET framework
- **C# 12.0** - Modern C# features with nullable reference types
- **Entity Framework Core 10.0** - ORM for database operations
- **SQL Server LocalDB** - Lightweight database engine

### Patterns & Practices
- **MVVM (Model-View-ViewModel)** - Clean separation of concerns
- **Repository Pattern** - Abstraction layer for data access
- **Dependency Injection** - Loose coupling between components
- **Async/Await** - Non-blocking database operations

##  Architecture

### Design Patterns

#### MVVM Pattern
```
View (XAML) ←→ ViewModel ←→ Model
                   ↓
              DatabaseService
                   ↓
              Entity Framework
                   ↓
              SQL Server
```

#### Key Components
- **Views**: `MainWindow`, `PlayerSelectionWindow`, `StatisticsWindow`
- **ViewModels**: `GameViewModel` with `INotifyPropertyChanged`
- **Models**: `Player`, `GameResult`
- **Services**: `DatabaseService` for data operations
- **Engine**: `GameEngine` for game logic

### Database Schema

```sql
Players
├── Id (PK)
├── Name
└── GameResults (Navigation)

GameResults
├── Id (PK)
├── PlayerX (FK)
├── PlayerO (FK)
├── Winner
├── Duration
└── PlayedAt
```

##  Getting Started

### Prerequisites

- **Windows 10/11**
- **.NET 10.0 SDK** - [Download](https://dotnet.microsoft.com/download)
- **Visual Studio 2022** (or later) with:
  - .NET desktop development workload
  - SQL Server Express LocalDB

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/tic-tac-toe-wpf.git
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

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run --project Tic-Tac-Toe
   ```

### Alternative: Visual Studio

1. Open `Tic-Tac-Toe.slnx` in Visual Studio 2022
2. Press `F5` to build and run
3. The database will be created automatically on first run

## Database

### Connection String
```csharp
Server=(localdb)\\mssqllocaldb;Database=TikTakToe;Trusted_Connection=True;
```

### Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName --project Tic-Tac-Toe
```

Update database:
```bash
dotnet ef database update --project Tic-Tac-Toe
```

Remove last migration:
```bash
dotnet ef migrations remove --project Tic-Tac-Toe
```

##  Project Structure

```
Tic-Tac-Toe/
├── DbContext/
│   ├── GameContext.cs          # EF Core DbContext
│   └── DbContextFactory.cs     # Design-time factory
├── Models/
│   ├── Player.cs               # Player entity
│   └── GameResult.cs           # Game result entity
├── ViewModels/
│   ├── GameViewModel.cs        # Main game logic
│   ├── ObjectObserver.cs       # Base class for INPC
│   └── RelayCommand.cs         # ICommand implementation
├── Views/
│   ├── MainWindow.xaml         # Game board UI
│   ├── PlayerSelectionWindow.xaml
│   └── StatisticsWindow.xaml   # Stats display
├── Services/
│   └── DatabaseService.cs      # Data access layer
├── RepositoryPattern/
│   └── RepositoryPattern.cs    # Generic repository
├── Migrations/                 # EF Core migrations
├── GameEngine.cs               # Core game logic
└── App.xaml                    # Application entry point
```

##  Key Features Explained

### 1. Player Management
- Automatic player registration on first use
- Name-based player lookup and creation
- Support for both human players and bot opponents

### 2. Game Logic
- Win detection using pattern matching (8 win conditions)
- Draw detection when board is full
- Turn management with player switching
- Board state management

### 3. Statistics System
- Total games played tracking
- Win/Loss/Draw counting
- Win rate percentage calculation
- Game history with timestamps
- Duration tracking for each game

### 4. Async Operations
- Non-blocking database operations
- UI thread safety with proper `async`/`await` usage
- Prevents UI freezing during data access

##  Future Enhancements

- [ ] **AI Implementation** - Actually implement the bot difficulty levels
- [ ] **Online Multiplayer** - Play against remote opponents
- [ ] **Themes** - Dark mode and custom color schemes
- [ ] **Sound Effects** - Audio feedback for moves and wins
- [ ] **Animations** - Smooth transitions and winning line highlights
- [ ] **Leaderboard** - Global rankings and achievements
- [ ] **Undo/Redo** - Take back moves
- [ ] **Game Replay** - Watch previous games
- [ ] **Export Statistics** - Generate reports as PDF/Excel
- [ ] **Localization** - Multi-language support

##  Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

##  License

This project is licensed under the MIT License

##  Author
- GitHub: [@Granuch](https://github.com/Granuch)
<!-- - LinkedIn: [Your Name](https://linkedin.com/in/yourprofile) -->

##  Acknowledgments

- Built as a learning project to demonstrate WPF and Entity Framework skills
- Classic Tic-Tac-Toe game rules
- Modern C# best practices and patterns

---

⭐ **Star this repository if you found it helpful!**
