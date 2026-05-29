namespace HypothesisGenerator.Models
{
    public class HypothesisRequest
    {
        public string Topic { get; set; } = string.Empty;

        public string DifficultyFilter { get; set; } = "Mixed";
    }
}