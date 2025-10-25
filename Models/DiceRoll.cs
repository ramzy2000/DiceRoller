using SQLite;

namespace DiceRoller.App.Models;

/// <summary>
/// Represents a single dice roll with its result and timestamp
/// </summary>
public class DiceRoll
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Number of sides on the die (1-100)
    /// </summary>
    public int Sides { get; set; }

    /// <summary>
    /// The result of the roll (1 to Sides)
    /// </summary>
    public int Result { get; set; }

    /// <summary>
    /// When the roll occurred
    /// </summary>
    public DateTime Timestamp { get; set; }

    public DiceRoll()
    {
        Timestamp = DateTime.UtcNow;
    }
}
