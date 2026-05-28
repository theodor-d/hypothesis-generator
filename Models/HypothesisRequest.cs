// ============================================================
// HypothesisRequest.cs — Data the user sends to us
// ============================================================

namespace HypothesisGenerator.Models
{
    public class HypothesisRequest
    {
        // The research topic the user typed in (e.g. "social media and sleep")
        public string Topic { get; set; } = string.Empty;

        // Optional difficulty filter: "Beginner", "Intermediate", "Advanced", or "Mixed"
        public string DifficultyFilter { get; set; } = "Mixed";
    }
}