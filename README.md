# 🎲 Dice Roller - .NET MAUI Blazor Hybrid App

A cross-platform dice rolling application built with .NET MAUI Blazor Hybrid, supporting dice from d1 to d100 with persistent roll history.

## 🌟 Features

- **Dice Rolling**: Roll any die from d1 to d100
- **Common Dice Support**: Quick access to standard gaming dice (d4, d6, d8, d10, d12, d20, d100)
- **Custom Dice**: Enter any number of sides between 1 and 100
- **Roll History**: All rolls are saved to SQLite database
- **Beautiful UI**: Modern, responsive interface with animations
- **Cross-Platform**: Runs on Android, iOS, macOS, and Windows

## 🏗️ Architecture

The application follows clean architecture principles with clear separation of concerns:

### Layers

1. **UI Layer** (`Components/`)
   - Blazor components for user interface
   - DiceRoller.razor: Main dice rolling interface
   - Responsive and adaptive layout

2. **Business Logic Layer** (`Services/`)
   - `IDiceRollerService`: Interface for dice rolling logic
   - `DiceRollerService`: Implementation of random dice rolling (1-100 sides)
   - `IDiceGameService`: Interface for game operations with persistence
   - `DiceGameService`: Combines rolling with database operations

3. **Data Layer** (`Data/`)
   - `DiceRollerDbContext`: SQLite database context
   - Manages roll history persistence
   - CRUD operations for dice rolls

4. **Models** (`Models/`)
   - `DiceRoll`: Entity representing a single dice roll

### Database Schema

```sql
CREATE TABLE DiceRoll (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Sides INTEGER NOT NULL,
    Result INTEGER NOT NULL,
    Timestamp DATETIME NOT NULL
);
```

## 🚀 Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio 2022 (17.8+) or VS Code with C# Dev Kit
- Platform-specific requirements:
  - **Android**: Android SDK 24.0+
  - **iOS/macOS**: Xcode 15.0+ (macOS only)
  - **Windows**: Windows 10 version 1809+ with Windows SDK 19041+

### Installation

1. Clone the repository:
```bash
git clone https://github.com/ramzy2000/DiceRoller.git
cd DiceRoller
```

2. Install .NET MAUI workload (if not already installed):
```bash
# For Android
dotnet workload install maui-android

# For other platforms (on respective OS)
dotnet workload install maui-ios      # macOS only
dotnet workload install maui-maccatalyst  # macOS only
dotnet workload install maui-windows  # Windows only
```

3. Restore dependencies:
```bash
dotnet restore
```

### Building

#### Android
```bash
dotnet build -f net9.0-android
```

#### iOS (macOS only)
```bash
dotnet build -f net9.0-ios
```

#### macOS (macOS only)
```bash
dotnet build -f net9.0-maccatalyst
```

#### Windows (Windows only)
```bash
dotnet build -f net9.0-windows10.0.19041.0
```

### Running

Run the app for your target platform:

```bash
# Android (requires emulator or connected device)
dotnet build -t:Run -f net9.0-android

# iOS (macOS only, requires simulator or device)
dotnet build -t:Run -f net9.0-ios

# macOS (macOS only)
dotnet build -t:Run -f net9.0-maccatalyst

# Windows (Windows only)
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

## 🧪 Testing

The project includes unit tests for core functionality:

- `DiceRollerServiceTests`: Tests for dice rolling logic
  - Valid roll ranges
  - Common dice types
  - Edge cases (min/max sides)
  - Invalid input handling
  - Randomness validation

- `DiceRollerDbContextTests`: Tests for database operations
  - Save rolls
  - Retrieve history
  - Clear history
  - Roll counting

### Running Tests

```bash
dotnet test
```

## 📦 Dependencies

- **Microsoft.Maui.Controls**: MAUI framework
- **Microsoft.AspNetCore.Components.WebView.Maui**: Blazor integration
- **sqlite-net-pcl**: SQLite ORM
- **SQLitePCLRaw.bundle_green**: SQLite native binaries
- **xunit**: Unit testing framework (dev dependency)

## 🎨 User Interface

The application features:
- Clean, modern design with gradient colors
- Quick-select buttons for common dice (d4, d6, d8, d10, d12, d20, d100)
- Custom dice input field for any value 1-100
- Large, animated result display
- Scrollable roll history with timestamps
- Responsive layout for all screen sizes

## 📱 Platform Support

| Platform | Supported | Min Version |
|----------|-----------|-------------|
| Android  | ✅ | API 24 (7.0) |
| iOS      | ✅ | 15.0 |
| macOS    | ✅ | 15.0 (Mac Catalyst) |
| Windows  | ✅ | Windows 10 (1809+) |

## 🔮 Future Enhancements

Potential improvements for future versions:
- Multiple dice rolling (e.g., "Roll 3d6")
- Dice roll modifiers (+/-  values)
- Statistics and analytics
- Custom themes and color schemes
- Roll presets/favorites
- Export roll history
- Dice sound effects
- 3D dice animations
- Sharing results
- Dark mode support

## 📄 License

This project is part of a demonstration and learning exercise.

## 👤 Author

Built as part of a .NET MAUI Blazor Hybrid development exercise following SDLC best practices.

## 🙏 Acknowledgments

- Built with .NET MAUI and Blazor
- SQLite for local data persistence
- xUnit for testing framework

