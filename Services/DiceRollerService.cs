namespace DiceRoller.App.Services;

/// <summary>
/// Service interface for rolling dice
/// </summary>
public interface IDiceRollerService
{
    /// <summary>
    /// Rolls a die with the specified number of sides
    /// </summary>
    /// <param name="sides">Number of sides (1-100)</param>
    /// <returns>The result of the roll</returns>
    int Roll(int sides);

    /// <summary>
    /// Gets the minimum number of sides supported
    /// </summary>
    int MinSides { get; }

    /// <summary>
    /// Gets the maximum number of sides supported
    /// </summary>
    int MaxSides { get; }
}

/// <summary>
/// Implementation of dice rolling logic
/// </summary>
public class DiceRollerService : IDiceRollerService
{
    private readonly Random _random;

    public int MinSides => 1;
    public int MaxSides => 100;

    public DiceRollerService()
    {
        _random = new Random();
    }

    public int Roll(int sides)
    {
        if (sides < MinSides || sides > MaxSides)
        {
            throw new ArgumentOutOfRangeException(nameof(sides), 
                $"Sides must be between {MinSides} and {MaxSides}");
        }

        return _random.Next(1, sides + 1);
    }
}
