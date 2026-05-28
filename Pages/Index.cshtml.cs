// ============================================================
// Index.cshtml.cs — Code-behind for the home page
// Handles form submission and calls GeminiService
// ============================================================

using HypothesisGenerator.Models;
using HypothesisGenerator.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HypothesisGenerator.Pages
{
    public class IndexModel : PageModel
    {
        // GeminiService is injected automatically via the constructor
        private readonly GeminiService _geminiService;
        private readonly ILogger<IndexModel> _logger;

        // [BindProperty] means ASP.NET will automatically fill this
        // from the form POST data (the <input name="Topic"> etc.)
        [BindProperty]
        public HypothesisRequest Request { get; set; } = new();

        // Error message to display on the page if something goes wrong
        public string ErrorMessage { get; set; } = string.Empty;

        public IndexModel(GeminiService geminiService, ILogger<IndexModel> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }

        // Called when the page loads normally (GET request)
        public void OnGet()
        {
            // Nothing to do on first load — just show the empty form
        }

        // Called when the form is submitted (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // Check if the form data is valid (e.g., Topic is not empty)
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please enter a research topic.";
                return Page(); // Stay on the same page and show error
            }

            // Clean up the input: trim whitespace
            Request.Topic = Request.Topic.Trim();

            if (string.IsNullOrWhiteSpace(Request.Topic))
            {
                ErrorMessage = "Please enter a research topic.";
                return Page();
            }

            try
            {
                // Call Gemini API to generate hypotheses
                _logger.LogInformation("Generating hypotheses for topic: {Topic}", Request.Topic);
                var results = await _geminiService.GenerateHypothesesAsync(Request);

                // Store results in TempData so the Results page can read them
                // TempData only survives one redirect, which is exactly what we need
                TempData["Topic"] = Request.Topic;
                TempData["DifficultyFilter"] = Request.DifficultyFilter;
                TempData["ResultsJson"] = Newtonsoft.Json.JsonConvert.SerializeObject(results);

                // Redirect to the Results page
                return RedirectToPage("/Results");
            }
            catch (InvalidOperationException ex)
            {
                // This is thrown if the API key is not set
                ErrorMessage = ex.Message;
                _logger.LogError(ex, "API key configuration error");
                return Page();
            }
            catch (HttpRequestException ex)
            {
                // This is thrown if the API call fails
                ErrorMessage = $"Could not reach Gemini API: {ex.Message}";
                _logger.LogError(ex, "HTTP request to Gemini failed");
                return Page();
            }
            catch (Exception ex)
            {
                // Catch-all for unexpected errors
                ErrorMessage = "Something went wrong. Please try again.";
                _logger.LogError(ex, "Unexpected error generating hypotheses");
                return Page();
            }
        }
    }
}