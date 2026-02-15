using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services
{
    public class Chat : IOpenAi
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public Chat(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OPENAI_API_KEY"];
            Console.WriteLine($"API Key: {_apiKey}");

        }
        public async Task<string> chatGpt(string question)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _apiKey);

            // בניית הבקשה לפי Chat Completions API
            var body = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
            new { role = "user", content = question }
        }
            };
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            int retries = 3;

            for (int i = 0; i < retries; i++)
            {
                try
                {
                    var response = await _httpClient.PostAsync(
                        "https://api.openai.com/v1/chat/completions", content);

                    // הדפס פרטי response
                    Console.WriteLine($"Status Code: {(int)response.StatusCode} ({response.StatusCode})");
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
                    }

                    var bodyContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Body: {bodyContent}");

                    // אם הצלחנו, מחזירים את התשובה
                    response.EnsureSuccessStatusCode();

                    using var doc = JsonDocument.Parse(bodyContent);
                    var text = doc.RootElement
                                  .GetProperty("choices")[0]
                                  .GetProperty("message")
                                  .GetProperty("content")
                                  .GetString();

                    return text;
                }
                catch (HttpRequestException ex) when ((int?)ex.StatusCode == 429)
                {
                    // המתנה לפני retry
                    await Task.Delay(1000 * (i + 1));
                }
            }

            throw new Exception("Request failed after multiple retries due to 429 Too Many Requests.");
        }



    }
}
