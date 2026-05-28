// ============================================================
// GeminiService.cs — Now using Groq API
// ============================================================
using HypothesisGenerator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HypothesisGenerator.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private const string GroqApiUrl =
            "https://api.groq.com/openai/v1/chat/completions";

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<List<HypothesisResult>> GenerateHypothesesAsync(HypothesisRequest request)
        {
            var apiKey = _configuration["GroqApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Groq API key is not set. Please add it to appsettings.json");

            var prompt = BuildPrompt(request);

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.8,
                max_tokens = 2048
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(GroqApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Groq API Error: {response.StatusCode} - {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return ParseGroqResponse(responseBody);
        }

        public async Task<HypothesisResult> GenerateOneHypothesisAsync(HypothesisRequest request)
        {
            var apiKey = _configuration["GroqApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("Groq API key is not set. Please add it to appsettings.json");

            var prompt = $@"
You are a research methods professor helping university students.

Research Topic: ""{request.Topic}""

Generate exactly 1 research hypothesis at {request.DifficultyFilter} difficulty level.

Return ONLY a valid JSON object, no markdown, no explanation, just raw JSON:
{{
  ""Statement"": ""hypothesis here"",
  ""Difficulty"": ""{request.DifficultyFilter}"",
  ""Methodology"": ""e.g. Survey"",
  ""Explanation"": ""one sentence here""
}}
";

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = 0.9,
                max_tokens = 512
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync(GroqApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Groq API Error: {response.StatusCode} - {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                var responseObj = JObject.Parse(responseBody);
                var text = responseObj["choices"]?[0]?["message"]?["content"]?.ToString() ?? "";

                text = text.Trim();
                if (text.StartsWith("```json")) text = text[7..];
                if (text.StartsWith("```")) text = text[3..];
                if (text.EndsWith("```")) text = text[..^3];
                text = text.Trim();

                return JsonConvert.DeserializeObject<HypothesisResult>(text)
                       ?? GetFallbackHypotheses()[0];
            }
            catch
            {
                return GetFallbackHypotheses()[0];
            }
        }

        private string BuildPrompt(HypothesisRequest request)
        {
            var difficultyInstruction = request.DifficultyFilter == "Mixed"
                ? "Use a mix of Beginner (2), Intermediate (2), and Advanced (1) difficulty levels."
                : $"All 5 hypotheses should be {request.DifficultyFilter} difficulty level.";

            return @"
You are a research methods professor helping university students.

Research Topic: """ + request.Topic + @"""

Generate exactly 5 research hypotheses.
" + difficultyInstruction + @"

For each hypothesis, provide:
1. A clear, testable hypothesis statement
2. Difficulty level: ""Beginner"", ""Intermediate"", or ""Advanced""
3. Suggested methodology (e.g. Survey, Experiment, Case Study, etc.)
4. One-sentence explanation why it's worth testing

Return ONLY valid JSON array, no markdown, no explanation, just the raw JSON:
[
  {
    ""Statement"": ""hypothesis here"",
    ""Difficulty"": ""Beginner"",
    ""Methodology"": ""Survey"",
    ""Explanation"": ""one sentence here""
  }
]
";
        }

        private List<HypothesisResult> ParseGroqResponse(string responseBody)
        {
            try
            {
                var responseObj = JObject.Parse(responseBody);
                var text = responseObj["choices"]?[0]?["message"]?["content"]?.ToString();

                if (string.IsNullOrEmpty(text))
                    return GetFallbackHypotheses();

                text = text.Trim();
                if (text.StartsWith("```json")) text = text[7..];
                if (text.StartsWith("```")) text = text[3..];
                if (text.EndsWith("```")) text = text[..^3];
                text = text.Trim();

                var results = JsonConvert.DeserializeObject<List<HypothesisResult>>(text);
                return results ?? GetFallbackHypotheses();
            }
            catch
            {
                return GetFallbackHypotheses();
            }
        }

        private List<HypothesisResult> GetFallbackHypotheses()
        {
            return new List<HypothesisResult>
            {
                new HypothesisResult
                {
                    Statement = "⚠️ Cannot connect to AI at the moment.",
                    Difficulty = "N/A",
                    Methodology = "N/A",
                    Explanation = "Please check your internet connection and API key."
                }
            };
        }
    }
}