using BapDb;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using BapShared;

namespace BapWeb;

public interface IGameDataSaver
{
    Task<List<(string difficulty, string difficultyDescription)>> GetCurrentScoreBoards();
    Task<List<Score>> GetScores(string difficulty, int topScoresToTake = 10, bool higherScoreIsBetter = true);
    Task<Score> AddScore(Score newScore);
    Task<T?> GetGameStorage<T>();
    Task<bool> UpdateGameStorage<T>(T itemToSave);
    Task<List<Score>> GetScoresWithNewScoreIfWarranted(Score newScore, int topScoresToTake = 10, bool higherScoreIsBetter = true);
}
public interface IGameDataSaver<TGameDesc> : IGameDataSaver where TGameDesc : IGameDescription
{


}

public class DefaultGameDataSaver<TGameDesc> : IGameDataSaver<TGameDesc> where TGameDesc : IGameDescription
{

    private IGameDescription _gameDescription { get; set; }
    private ButtonContext _db { get; set; }
    public DefaultGameDataSaver(TGameDesc gameDesc, ButtonContext buttonContext)
    {
        _gameDescription = gameDesc;
        _db = buttonContext;
    }

    /// <summary>
    /// This gets  a list of all of the difficultys and the difficulty descriptions from the database. 
    /// </summary>
    /// <returns></returns>
    /// Todo try turning this into a record so comparison is easier.
    public async Task<List<(string difficulty, string difficultyDescription)>> GetCurrentScoreBoards()
    {
        List<string> difficulties = await _db.Scores.Select(t => t.Difficulty).ToListAsync();
        List<(string difficulty, string difficultyDescription)> results = new List<(string difficulty, string difficultyDescription)>();
        foreach (var item in difficulties)
        {
            string details = _db.Scores.OrderByDescending(t => t.ScoreId).First(t => t.Difficulty == item).DifficultyDetails;
            results.Add((item, details));
        }
        return results;
    }

    /// <summary>
    /// This gets a string of data that was saved for the app. This could be serialized text, Json, or yaml or xml or anythign else. It is up to the game to decide what goes here.
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetGameStorage()
    {
        return (await _db.GameStorageVault.Where(t => t.GameUniqueId == _gameDescription.UniqueId).FirstOrDefaultAsync())?.Data ?? "";
    }


    public async Task<bool> UpdateGameStorage<T>(T itemToSave)
    {
        string textToSave = JsonSerializer.Serialize<T>(itemToSave);
        await UpdateGameStorage(textToSave);
        return true;
    }
    public async Task<T?> GetGameStorage<T>()
    {
        string savedText = await GetGameStorage();
        if (string.IsNullOrEmpty(savedText))
        {
            return default(T);
        }
        T? savedObject = JsonSerializer.Deserialize<T>(savedText);
        return savedObject;
    }

    public async Task<bool> UpdateGameStorage(string data)
    {
        var currentGameStorage = await _db.GameStorageVault.Where(t => t.GameUniqueId == _gameDescription.UniqueId).FirstOrDefaultAsync();
        if (currentGameStorage == null)
        {
            _db.GameStorageVault.Add(new GameStorage() { GameUniqueId = _gameDescription.UniqueId, Data = data });
        }
        else
        {
            currentGameStorage.Data = data;
        }
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Score>> GetScores(string difficulty, int topScoresToTake = 10, bool higherScoreIsBetter = true)
    {
        var query = _db.Scores.Where(t => t.GameId == _gameDescription.UniqueId && t.ScoringModelVersion == _gameDescription.ScoringModelVersion && t.Difficulty == difficulty);
        if (higherScoreIsBetter)
        {
            query = query.OrderByDescending(t => t.NormalizedScore);
        }
        else
        {
            query = query.OrderBy(t => t.NormalizedScore);
        }
        return await query.Take(topScoresToTake).ToListAsync();
    }

    public async Task<List<Score>> GetScoresWithNewScoreIfWarranted(Score newScore, int topScoresToTake = 10, bool higherScoreIsBetter = true)
    {
        List<Score> currentScores = await GetScores(newScore.Difficulty, topScoresToTake, higherScoreIsBetter);
        if (newScore != null)
        {

            Score? currentBottomScore = currentScores.LastOrDefault();
            if (currentScores.Count < 10 || currentBottomScore == null || newScore.NormalizedScore > currentBottomScore.NormalizedScore)
            {
                currentScores.Add(newScore);
                if (currentScores.Count > 10 && currentBottomScore != null)
                {
                    currentScores.Remove(currentBottomScore);
                }
            }

        }
        return currentScores;
    }

    public async Task<Score> AddScore(Score score)
    {
        score.ScoringModelVersion = _gameDescription.ScoringModelVersion;
        score.GameId = _gameDescription.UniqueId;

        _db.Scores.Add(score);
        await _db.SaveChangesAsync();
        return score;
    }


}

