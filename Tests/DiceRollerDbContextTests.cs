using DiceRoller.App.Data;
using DiceRoller.App.Models;
using Xunit;

namespace DiceRoller.App.Tests;

/// <summary>
/// Unit tests for DiceRollerDbContext
/// </summary>
public class DiceRollerDbContextTests : IDisposable
{
    private readonly DiceRollerDbContext _context;
    private readonly string _testDbPath;

    public DiceRollerDbContextTests()
    {
        // Create a unique test database for each test
        _testDbPath = Path.Combine(Path.GetTempPath(), $"test_diceroller_{Guid.NewGuid()}.db3");
        _context = new DiceRollerDbContext(_testDbPath);
    }

    public void Dispose()
    {
        // Clean up test database
        if (File.Exists(_testDbPath))
        {
            File.Delete(_testDbPath);
        }
    }

    [Fact]
    public async Task SaveRollAsync_SavesRollToDatabase()
    {
        // Arrange
        var roll = new DiceRoll
        {
            Sides = 6,
            Result = 4,
            Timestamp = DateTime.UtcNow
        };

        // Act
        await _context.SaveRollAsync(roll);
        var rolls = await _context.GetAllRollsAsync();

        // Assert
        Assert.Single(rolls);
        Assert.Equal(6, rolls[0].Sides);
        Assert.Equal(4, rolls[0].Result);
    }

    [Fact]
    public async Task GetAllRollsAsync_ReturnsRollsInDescendingOrder()
    {
        // Arrange
        var roll1 = new DiceRoll { Sides = 6, Result = 3, Timestamp = DateTime.UtcNow.AddMinutes(-2) };
        var roll2 = new DiceRoll { Sides = 20, Result = 15, Timestamp = DateTime.UtcNow.AddMinutes(-1) };
        var roll3 = new DiceRoll { Sides = 8, Result = 7, Timestamp = DateTime.UtcNow };

        await _context.SaveRollAsync(roll1);
        await _context.SaveRollAsync(roll2);
        await _context.SaveRollAsync(roll3);

        // Act
        var rolls = await _context.GetAllRollsAsync();

        // Assert
        Assert.Equal(3, rolls.Count);
        Assert.Equal(8, rolls[0].Sides); // Most recent
        Assert.Equal(20, rolls[1].Sides);
        Assert.Equal(6, rolls[2].Sides); // Oldest
    }

    [Fact]
    public async Task GetRecentRollsAsync_ReturnsLimitedRolls()
    {
        // Arrange
        for (int i = 0; i < 10; i++)
        {
            await _context.SaveRollAsync(new DiceRoll
            {
                Sides = 6,
                Result = i + 1,
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        // Act
        var rolls = await _context.GetRecentRollsAsync(5);

        // Assert
        Assert.Equal(5, rolls.Count);
    }

    [Fact]
    public async Task ClearAllRollsAsync_RemovesAllRolls()
    {
        // Arrange
        await _context.SaveRollAsync(new DiceRoll { Sides = 6, Result = 3 });
        await _context.SaveRollAsync(new DiceRoll { Sides = 20, Result = 15 });

        // Act
        await _context.ClearAllRollsAsync();
        var rolls = await _context.GetAllRollsAsync();

        // Assert
        Assert.Empty(rolls);
    }

    [Fact]
    public async Task GetRollCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        await _context.SaveRollAsync(new DiceRoll { Sides = 6, Result = 3 });
        await _context.SaveRollAsync(new DiceRoll { Sides = 20, Result = 15 });
        await _context.SaveRollAsync(new DiceRoll { Sides = 8, Result = 7 });

        // Act
        var count = await _context.GetRollCountAsync();

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetRollCountAsync_WithNoRolls_ReturnsZero()
    {
        // Act
        var count = await _context.GetRollCountAsync();

        // Assert
        Assert.Equal(0, count);
    }
}
