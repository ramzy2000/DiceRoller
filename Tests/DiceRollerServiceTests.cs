using DiceRoller.App.Services;
using Xunit;

namespace DiceRoller.App.Tests;

/// <summary>
/// Unit tests for DiceRollerService
/// </summary>
public class DiceRollerServiceTests
{
    private readonly IDiceRollerService _service;

    public DiceRollerServiceTests()
    {
        _service = new DiceRollerService();
    }

    [Fact]
    public void Roll_WithValidSides_ReturnsValueInRange()
    {
        // Arrange
        int sides = 6;

        // Act
        int result = _service.Roll(sides);

        // Assert
        Assert.InRange(result, 1, sides);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(6)]
    [InlineData(8)]
    [InlineData(10)]
    [InlineData(12)]
    [InlineData(20)]
    [InlineData(100)]
    public void Roll_WithCommonDiceSides_ReturnsValidResult(int sides)
    {
        // Act
        int result = _service.Roll(sides);

        // Assert
        Assert.InRange(result, 1, sides);
    }

    [Fact]
    public void Roll_WithMinSides_ReturnsOne()
    {
        // Arrange
        int sides = _service.MinSides;

        // Act
        int result = _service.Roll(sides);

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public void Roll_WithMaxSides_ReturnsValueInRange()
    {
        // Arrange
        int sides = _service.MaxSides;

        // Act
        int result = _service.Roll(sides);

        // Assert
        Assert.InRange(result, 1, sides);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(101)]
    [InlineData(1000)]
    public void Roll_WithInvalidSides_ThrowsArgumentOutOfRangeException(int sides)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.Roll(sides));
    }

    [Fact]
    public void Roll_MultipleTimes_ProducesVariedResults()
    {
        // Arrange
        int sides = 20;
        var results = new HashSet<int>();

        // Act
        for (int i = 0; i < 100; i++)
        {
            results.Add(_service.Roll(sides));
        }

        // Assert - with 100 rolls of a d20, we should get at least 10 different values
        // This tests randomness (though not perfect, good enough for basic validation)
        Assert.True(results.Count >= 10, $"Expected at least 10 different values, got {results.Count}");
    }

    [Fact]
    public void MinSides_ReturnsOne()
    {
        // Assert
        Assert.Equal(1, _service.MinSides);
    }

    [Fact]
    public void MaxSides_ReturnsOneHundred()
    {
        // Assert
        Assert.Equal(100, _service.MaxSides);
    }
}
