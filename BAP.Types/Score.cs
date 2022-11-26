using BAP.Types;

namespace BAP.Types
{
    public class Score 
    {
        public int ScoreId { get; set; }
        public string GameId { get; set; } = "";
        public string DifficultyName { get; set; } = "";
        public string DifficultyDescription { get; set; } = "";
        public string UserName { get; set; } = "";
        public string ScoreDescription { get; set; } = "";
        public string ScoreData { get; set; } = "";
        public decimal NormalizedScore { get; set; }
    }
}