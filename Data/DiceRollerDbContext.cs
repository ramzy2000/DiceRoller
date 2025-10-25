using SQLite;
using DiceRoller.App.Models;

namespace DiceRoller.App.Data;

/// <summary>
/// SQLite database context for DiceRoller application
/// </summary>
public class DiceRollerDbContext
{
    private readonly SQLiteAsyncConnection _database;

    public DiceRollerDbContext(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<DiceRoll>().Wait();
    }

    /// <summary>
    /// Saves a dice roll to the database
    /// </summary>
    public Task<int> SaveRollAsync(DiceRoll roll)
    {
        return _database.InsertAsync(roll);
    }

    /// <summary>
    /// Gets all dice rolls ordered by timestamp descending
    /// </summary>
    public Task<List<DiceRoll>> GetAllRollsAsync()
    {
        return _database.Table<DiceRoll>()
            .OrderByDescending(r => r.Timestamp)
            .ToListAsync();
    }

    /// <summary>
    /// Gets a specific number of recent rolls
    /// </summary>
    public Task<List<DiceRoll>> GetRecentRollsAsync(int count)
    {
        return _database.Table<DiceRoll>()
            .OrderByDescending(r => r.Timestamp)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Deletes all dice rolls from the database
    /// </summary>
    public Task<int> ClearAllRollsAsync()
    {
        return _database.DeleteAllAsync<DiceRoll>();
    }

    /// <summary>
    /// Gets the total count of rolls
    /// </summary>
    public Task<int> GetRollCountAsync()
    {
        return _database.Table<DiceRoll>().CountAsync();
    }
}
