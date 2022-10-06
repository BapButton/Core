namespace BapDb
{
    public class Score
    {
        public int ScoreId { get; set; }
        public string GameId { get; set; } = "";
        public string ScoringModelVersion { get; set; } = "";
        public string Difficulty { get; set; } = "";
        public string DifficultyDetails { get; set; } = "";
        public string UserName { get; set; } = "";
        public decimal NormalizedScore { get; set; }
        public string ScoreDescription { get; set; } = "";
        public string ScoreFullDetails { get; set; } = "";
        public string ScoreData { get; set; } = "";
    }
}