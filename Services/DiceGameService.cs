using DiceRoller.App.Data;
using DiceRoller.App.Models;

namespace DiceRoller.App.Services;

/// <summary>
/// Service that combines dice rolling with history persistence
/// </summary>
public interface IDiceGameService
{
    /// <summary>
    /// Rolls a die and saves it to history
    /// </summary>
    Task<DiceRoll> RollAndSaveAsync(int sides);

    /// <summary>
    /// Gets all roll history
    /// </summary>
    Task<List<DiceRoll>> GetHistoryAsync();

    /// <summary>
    /// Gets recent rolls
    /// </summary>
    Task<List<DiceRoll>> GetRecentRollsAsync(int count);

    /// <summary>
    /// Clears all roll history
    /// </summary>
    Task ClearHistoryAsync();

    /// <summary>
    /// Gets total roll count
    /// </summary>
    Task<int> GetTotalRollsAsync();

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
/// Implementation of dice game service
/// </summary>
public class DiceGameService : IDiceGameService
{
    private readonly IDiceRollerService _diceRoller;
    private readonly DiceRollerDbContext _dbContext;

    public int MinSides => _diceRoller.MinSides;
    public int MaxSides => _diceRoller.MaxSides;

    public DiceGameService(IDiceRollerService diceRoller, DiceRollerDbContext dbContext)
    {
        _diceRoller = diceRoller;
        _dbContext = dbContext;
    }

    public async Task<DiceRoll> RollAndSaveAsync(int sides)
    {
        var result = _diceRoller.Roll(sides);
        var roll = new DiceRoll
        {
            Sides = sides,
            Result = result,
            Timestamp = DateTime.UtcNow
        };

        await _dbContext.SaveRollAsync(roll);
        return roll;
    }

    public Task<List<DiceRoll>> GetHistoryAsync()
    {
        return _dbContext.GetAllRollsAsync();
    }

    public Task<List<DiceRoll>> GetRecentRollsAsync(int count)
    {
        return _dbContext.GetRecentRollsAsync(count);
    }

    public Task ClearHistoryAsync()
    {
        return _dbContext.ClearAllRollsAsync();
    }

    public Task<int> GetTotalRollsAsync()
    {
        return _dbContext.GetRollCountAsync();
    }
}
