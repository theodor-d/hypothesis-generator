using HypothesisGenerator.Models;
using HypothesisGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HypothesisGenerator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly GeminiService _geminiService;
        private readonly GoogleSheetsService _sheetsService;
        private readonly ILogger<IndexModel> _logger;

        [BindProperty]
        public HypothesisRequest HypothesisInput { get; set; } = new();

        public string ErrorMessage { get; set; } = string.Empty;

        public IndexModel(GeminiService geminiService, GoogleSheetsService sheetsService, ILogger<IndexModel> logger)
        {
            _geminiService = geminiService;
            _sheetsService = sheetsService;
            _logger = logger;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please enter a research topic.";
                return Page();
            }

            HypothesisInput.Topic = HypothesisInput.Topic.Trim();

            if (string.IsNullOrWhiteSpace(HypothesisInput.Topic))
            {
                ErrorMessage = "Please enter a research topic.";
                return Page();
            }

            try
            {
                _logger.LogInformation("Generating hypotheses for topic: {Topic}", HypothesisInput.Topic);
                var results = await _geminiService.GenerateHypothesesAsync(HypothesisInput);

                // Log to Google Sheets only if user accepted cookies
                var cookieConsent = HttpContext.Request.Cookies["cookieConsent"];
                if (cookieConsent == "accepted")
                {
                    try
                    {
                        await _sheetsService.LogSearchAsync(
                            HypothesisInput.Topic,
                            HypothesisInput.DifficultyFilter,
                            HypothesisInput.Language
                        );
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to log to Google Sheets");
                    }
                }

                TempData["Topic"] = HypothesisInput.Topic;
                TempData["DifficultyFilter"] = HypothesisInput.DifficultyFilter;
                TempData["Language"] = HypothesisInput.Language;
                TempData["ResultsJson"] = Newtonsoft.Json.JsonConvert.SerializeObject(results);

                return RedirectToPage("/Results");
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "API key configuration error");
                return Page();
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"Could not reach Groq API: {ex.Message}";
                _logger.LogError(ex, "HTTP request to Groq failed");
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Something went wrong. Please try again.";
                _logger.LogError(ex, "Unexpected error generating hypotheses");
                return Page();
            }
        }
    }
}