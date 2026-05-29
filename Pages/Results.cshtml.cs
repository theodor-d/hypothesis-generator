using HypothesisGenerator.Models;
using HypothesisGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace HypothesisGenerator.Pages
{
    public class ResultsModel : PageModel
    {
        private readonly GeminiService _geminiService;
        private readonly ILogger<ResultsModel> _logger;

        public string Topic { get; set; } = string.Empty;
        public string DifficultyFilter { get; set; } = "Mixed";
        public List<HypothesisResult> Hypotheses { get; set; } = new();

        public ResultsModel(GeminiService geminiService, ILogger<ResultsModel> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            var topic = TempData["Topic"]?.ToString();
            var filter = TempData["DifficultyFilter"]?.ToString();
            var json = TempData["ResultsJson"]?.ToString();

            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(topic))
                return RedirectToPage("/Index");

            Topic = topic;
            DifficultyFilter = filter ?? "Mixed";
            Hypotheses = JsonConvert.DeserializeObject<List<HypothesisResult>>(json)
                         ?? new List<HypothesisResult>();

            return Page();
        }

        public async Task<IActionResult> OnPostRegenerateOneAsync(
            int index,
            string topic,
            string difficultyFilter,
            string hypothesesJson)
        {
            var hypotheses = JsonConvert.DeserializeObject<List<HypothesisResult>>(hypothesesJson)
                             ?? new List<HypothesisResult>();

            var targetDifficulty = hypotheses.ElementAtOrDefault(index)?.Difficulty ?? difficultyFilter;

            try
            {
                var request = new HypothesisRequest
                {
                    Topic = topic,
                    DifficultyFilter = targetDifficulty == "N/A" ? difficultyFilter : targetDifficulty
                };

                var newOne = await _geminiService.GenerateOneHypothesisAsync(request);
                hypotheses[index] = newOne;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to regenerate hypothesis at index {Index}", index);
            }

            TempData["Topic"] = topic;
            TempData["DifficultyFilter"] = difficultyFilter;
            TempData["ResultsJson"] = JsonConvert.SerializeObject(hypotheses);

            return RedirectToPage("/Results");
        }
    }
}