using BAP.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Types
{
	public interface IGameDataSaver
	{
		Task<List<(string difficulty, string difficultyDescription)>> GetCurrentScoreBoards();
		Task<List<Score>> GetScores(string difficulty, int topScoresToTake = 10, bool higherScoreIsBetter = true);
		Task<Score> AddScore(Score newScore);
		Task<T?> GetGameStorage<T>();
		Task<bool> UpdateGameStorage<T>(T itemToSave);
		Task<List<Score>> GetScoresWithNewScoreIfWarranted(Score newScore, int topScoresToTake = 10, bool higherScoreIsBetter = true);
	}
	public interface IGameDataSaver<TGameDesc> : IGameDataSaver where TGameDesc : IBapGameDescription
	{


	}
}
