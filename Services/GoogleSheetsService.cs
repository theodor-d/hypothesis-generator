using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace HypothesisGenerator.Services
{
    public class GoogleSheetsService
    {
        private readonly SheetsService _sheetsService;
        private const string SpreadsheetId = "1NSqT1JgxHx50mLTta7iP-gvrkLM-Tb4NzLubG3sCI2s";
        private const string Sheet = "Sheet1";

        public GoogleSheetsService(IConfiguration configuration)
        {
            var credentialsJson = configuration["GoogleCredentials"];

            var credential = GoogleCredential
                .FromJson(credentialsJson)
                .CreateScoped(SheetsService.Scope.Spreadsheets);

            _sheetsService = new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "HypothesisGenerator"
            });
        }

        public async Task LogSearchAsync(string topic, string difficulty, string language)
        {
            var range = $"{Sheet}!A:D";
            var valueRange = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object>
                    {
                        TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Asia/Bangkok").ToString("yyyy-MM-dd HH:mm:ss"),
                        topic,
                        difficulty,
                        language
                    }
                }
            };

            var request = _sheetsService.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            await request.ExecuteAsync();
        }
    }
}