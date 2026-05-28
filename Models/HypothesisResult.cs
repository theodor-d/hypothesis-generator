// ============================================================
// HypothesisResult.cs — One generated hypothesis
// ============================================================

namespace HypothesisGenerator.Models
{
    public class HypothesisResult
    {
        // The hypothesis statement itself (e.g. "Students who use social media...")
        public string Statement { get; set; } = string.Empty;

        // Difficulty: Beginner, Intermediate, or Advanced
        public string Difficulty { get; set; } = string.Empty;

        // Suggested research method (e.g. "Survey", "Experiment", "Case Study")
        public string Methodology { get; set; } = string.Empty;

        // A one-sentence explanation of why this hypothesis is interesting
        public string Explanation { get; set; } = string.Empty;
    }
}